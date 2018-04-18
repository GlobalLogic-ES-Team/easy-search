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
        var searchText = $("#srch-term").val();
        $.ajax({
            url: "Api/ElasticApi/GetbyZipcode",
            data: { SearchString: searchText },
            type: "POST",
            dataType: 'json',
            success: function (result) {
                try {
                    alert('success');

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
        $.ajax({
            url: "Api//ElasticApi/GetLocationDetail",
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
                else
                {
                    alert('something went wrong');
                }
            }
        });
    },
};