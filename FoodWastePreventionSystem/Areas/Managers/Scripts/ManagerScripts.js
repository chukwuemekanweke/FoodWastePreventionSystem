/// <reference path="../assets/js/jquery.min.js" />


$(document).ready(function () {



    function getElement(value) {
        if (value.indexOf('#') == -1) {
            value = value.slice(1);
            console.log(value);
            return document.getElementById(value.trim);
        }
        else if (value.indexOf('.' == -1)) {
            value = value.slice(1);
            console.log(value);
            return document.getElementsByClassName(value);
        }
        else {
            return document.getElementsByTagName(value)
        }
    }


    var element = document.getElementById('sel1');
    element.addEventListener('change', function () {
        var selectedCategory = this.value;
        if (selectedCategory === 'New') {
            document.getElementById('newCategory').disabled = false;
        } else {
            document.getElementById('newCategory').disabled = true;

        }

    }, false);

    document.getElementById('fileUploader').addEventListener('change', function (e) {
        
        var imagePlaceholder = document.getElementById('imagePlaceholder');
       
        while (imagePlaceholder.hasChildNodes())
        {
            imagePlaceholder.removeChild(imagePlaceholder.firstChild);
        }

        console.log(imagePlaceholder.childNodes);
        console.log(imagePlaceholder.childNodes.length);

        for(var i = 0; i<e.target.files.length;++i)
        {
            var image = document.createElement('img');
            image.setAttribute('class','img-responsive col-sm-2')
            image.src = URL.createObjectURL(e.target.files[i]);
            imagePlaceholder.appendChild(image);
        }

    }, false);





    //$('#sel1').change(function () {
    //    var categorySelection = $(this).val();
    //    if(categorySelection==='New')
    //    {
    //        $('newCategory').prop("disabled", false);
    //    }
    //});


});

