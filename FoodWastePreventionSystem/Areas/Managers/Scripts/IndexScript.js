$(document).ready(function () {

    AddEventForBlacklistAndUnblacklist();



});

var Path = "/Managers/Products/"

function AddEventForBlacklistAndUnblacklist() {
    addEventListenerList(document.getElementsByClassName('blacklist'), 'click', function (e) {
        e.preventDefault
        toggleBlacklist("blacklist", this);

    }, true);
    addEventListenerList(document.getElementsByClassName('unblacklist'), 'click', function (e) {
        e.preventDefault
        toggleBlacklist("unblacklist", this);

    }, true);
}


function toggleBlacklist(state, btnElement) {

    var formElement = btnElement.parentNode;
    var rowRecord = btnElement.parentNode.parentNode.parentNode;
    var stateTdElement = rowRecord.cells[4];
    var actionTdElement = rowRecord.cells[5];
    var stateElement = stateTdElement.firstChild;
    var productId = formElement.firstChild.value;
    var productName = formElement.firstChild.nextSibling.value


    console.log(rowRecord);
    console.log(stateTdElement);


    if (state === "blacklist") {

        if (confirm(`Are You Sure You Want To Blacklist ${productName}`)) {
            serverRequest(`${Path}BlacklistProductJSON`, "POST", JSON.stringify({ 'id': productId }), function (req) {
                alert(`${productName} Has Been Blacklisted Successfully`);
                stateElement.className = "badge btn-danger btn-sm"
                stateElement.textContent = "Inactive"
                actionTdElement.innerHTML = `<form action="/Managers/Products/Edit" method="post">` +
                                                `<input id="id" name="id" type="hidden" value="${productId}" />` +
                                                `<input id="name" name="name" type="hidden" value="${productName}" /> ` +
                                                '<button class="btn btn-default btn-xs unblacklist" type="button"><span class=" ion-arrow-return-left"></span></button>' +
                                                `<a class="btn btn-primary" href="/Managers/Products/Details/${productId}"><span class="fa fa-database"></span>Details</a>` +
                                            `</form>`;
                AddEventForBlacklistAndUnblacklist();
            }, function (req) {
                alert(`Error ${req.status}-${req.statusText}`);
            })
        }

    }
    else if (state === "unblacklist") {

        if (confirm(`Are You Sure You Want To Restore ${productName}`)) {
            serverRequest(`${Path}UnBlacklistProductJSON`, "POST", JSON.stringify({ 'id': productId }), function (req) {
                alert(`${productName} Has Been Restored Successfully`);
                stateElement.className = "badge btn-success btn-sm"
                stateElement.textContent = "Active"
                actionTdElement.innerHTML = `<form action="/Managers/Products/Edit" method="post">` +
                                                 `<input id="id" name="id" type="hidden" value="${productId}" />` +
                                                 `<input id="name" name="name" type="hidden" value="${productName}" /> ` +
                                                 `<button class="btn btn-default btn-xs" type="submit"><span class="fa fa-edit"></span></button>` +
                                                 `<button class="btn btn-default btn-xs blacklist" type="button"><span class="fa fa-times"></span></button>` +
                                                `<a class="btn btn-primary" href="/Managers/Products/Details/${productId}"><span class="fa fa-database"></span>Details</a>` +
                                                `</form> `
                                           ;
                AddEventForBlacklistAndUnblacklist();
            },
                function (req) {
                    alert(`Error ${req.status}-${req.statusText}`);
                });
        }

    }
}




function addEventListenerList(list, event, func) {
    console.log("im here");

    for (var i = 0; i < list.length; i++) {
        list[i].addEventListener(event, func, false);
    }
}

function serverRequest(url, method, body, successCallback, errorCallback) {
    var req = new XMLHttpRequest();
    req.open(method, url);
    req.setRequestHeader("Content-Type", "application/json");
    console.log(req);
    method === "POST" || method === "PUT" ? req.send(body) : req.send();

    req.onreadystatechange = function () {

        if (req.readyState === 4) {
            if (req.status != 200) {
                errorCallback(req)
            }
            else {
                successCallback(req)
            }
        }
    }
}

  