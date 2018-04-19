$(document).ajaxStart(function () {
    $("#processingBar").show();
});

$(document).ajaxStop(function () {
    $("#processingBar").hide();
});

$(document).ajaxError(function () {
    $("#processingBar").hide();
});



var Methods = {

    Search: function () {
        //debugger;
        var searchText = $("#srch-term").val();
        var isElastic = $("#isElastic")[0].checked;

        var url = "Api/SqlApi/GetbyZipcode";
        if (isElastic)
            url = "Api/ElasticApi/GetbyZipcode";

        $.ajax({
            url: url,
            data: { SearchString: searchText },
            type: "POST",
            dataType: 'json',
            success: function (result) {
                try {
                    $("#formatted_address").text(result.Formatted_Address);
                    $("#elapsed_Time").text(result.Performance.ElapsedTime);

                    $("#peoplelist").html('');
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist");

                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                alert('error');
            }
        });
    },

    GetAddress: function (lat, lng) {

        var isElastic = $("#isElastic")[0].checked;
        var url = "Api/SqlApi/GetLocationDetail";
        if (isElastic)
            url = "Api/ElasticApi/GetLocationDetail";

        $.ajax({
            url: url,
            async: true,
            data: { Lat: lat, Lng: lng },
            type: 'POST',
            success: function (result) {
                try {
                    $("#formatted_address").text(result.Formatted_Address);
                    $("#elapsed_Time").text(result.Performance.ElapsedTime);

                    $("#peoplelist").html('');
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist");

                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                if (ex.status == 415)
                    alert(ex.responseText);
                else {
                    alert('something went wrong');
                }
            }
        });
    },
};