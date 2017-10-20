using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class EmailLogic:Controller
    {
        IRepository<Store> StoresRepo { get; set; }
        BatchLogic BatchLogic { get; set; }

        ApplicationDbContext db { get; set; }



        public EmailLogic(IRepository<Store> _storesRepo, BatchLogic _batchL)
        {
            StoresRepo = _storesRepo;
            BatchLogic = _batchL;
            db = new ApplicationDbContext();
        }

        [AutomaticRetry(Attempts = 20)]
        public void PrepareMail()
        {
            List<StoreInformation> storesInformation = GetStoreAndManagerDetails();
            foreach (var storeInfo in storesInformation)
            {
                List<Batch> batches = BatchLogic.GetBatchesForAllProductsForStore(storeInfo.Store.Id);
                List<MailAddress> receipientsMailAddress = new List<MailAddress>();

                foreach (var record in storeInfo.StoreManagers)
                {
                    string receipientName = record.FirstName+" "+record.LastName;
                    if (!string.IsNullOrWhiteSpace(record.Email))
                    {
                        receipientsMailAddress.Add(new MailAddress(record.Email, receipientName));
                    }
                    
                }
               string content = ComposeMail(batches, storeInfo.Store);



                SendEmail(storeInfo.Store, receipientsMailAddress, "Inventory Information", content, "");

            }
        }

        private List<StoreInformation> GetStoreAndManagerDetails()
        {
            List<StoreInformation> storeInfo = new List<StoreInformation>();
            List<Store> stores = new List<Store>();
            StoresRepo.GetAll().ToList().ForEach(store =>
            {
                stores.Add(store);
            });

            foreach (var store in stores)
            {
                List<ApplicationUser> storeManagers = new List<ApplicationUser>();
                db.Users.Where(x => x.StoreId == store.Id).ToList().ForEach(user=> storeManagers.Add(user));
                storeInfo.Add(new StoreInformation()
                {
                    Store = store,
                    StoreManagers = storeManagers,

                });

            }

            return storeInfo;

        }

        public void SendEmail(Store store, List<MailAddress> emailList, string subject, string htmlContent, string plaintextContent)
        {
            if (emailList.Count > 0)
            {
                foreach (var receipient in emailList)
                {
                    try
                    {
                        MailMessage mail = new MailMessage();
                        mail.To.Add(receipient);


                        htmlContent = htmlContent.Replace("{{fullName}}", receipient.DisplayName);


                        //FileStream fileStream = new FileStream(HostingEnvironment.MapPath("~/Content/PdfReports/test.pdf"), FileMode.Open, FileAccess.Read);
                        //System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(fileStream, "testName.pdf", MediaTypeNames.Application.Pdf);
                        //mail.Attachments.Add(attachment);

                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        mail.Subject = subject;
                        mail.Body = htmlContent;
                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        //smtp.Host = "smtp.sendgrid.net";
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential
                        ("emekanweke604@gmail.com", "Live'4'Money");// Enter seders User name and password
                        mail.From = new MailAddress("noreply@wasteEat.com", "noreply@wasteEat.com");

                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                    catch
                    {
                        //take note of users who couldnt get email. manually send it to them later if background task retries fails
                    }

                }
            }
        }


        public string PopulateBody(Store store, string htmlContent, string path)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(HostingEnvironment.MapPath(path)))
            {
                body = reader.ReadToEnd();
            }
            //string imageUrl = HostingEnvironment.MapPath(store);

            //body = body.Replace("{{hospitalLogo}}", HostingEnvironment.MapPath(hospital.hospital_logo_url));
            body = body.Replace("{{storeName}}", store.Name);
            body = body.Replace("{{storeAddress}}", store.Address);
            body = body.Replace("{{tableRows}}", htmlContent);

            return body;
        }

        public string ComposeMail(List<Batch> batches, Store store)
        {
            StringWriter stringWriter = new StringWriter();


            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
               
                    foreach (var batch in batches)
                    {
                        writer.AddStyleAttribute(HtmlTextWriterStyle.MarginBottom, "10px");
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        writer.Write(batch.Product.Name);

                        writer.RenderEndTag();

                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        writer.Write(batch.ManufactureDate.ToShortDateString());

                        writer.RenderEndTag();

                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        writer.Write(batch.ExpiryDate.ToShortDateString());

                        writer.RenderEndTag();

                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        writer.Write(batch.QuantityPurchased);

                        writer.RenderEndTag();

                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        writer.Write(batch.QuantitySold);

                        writer.RenderEndTag();

                    writer.RenderEndTag();
                    }
                    return PopulateBody(store, stringWriter.ToString(), "~/Content/EmailTemplate.html");

                
               
              
               


            }

        }



    }
}