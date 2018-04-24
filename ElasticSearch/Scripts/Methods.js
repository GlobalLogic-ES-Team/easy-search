
var Methods = {

    Search: function () {


        var searchText = $("#srch-term").val();

        $("#peoplelist_sql").html('');
        $("#peoplelist_es").html('');
        $("#elapsed_Time_sql").text("na");
        $("#elapsed_Time_es").text("na");
        $("#count_sql").text("");
        $("#count_es").text("");

        $("#title_sql").text("Sql");
        $("#title_es").text("ElasticSearch");
        

        $.ajax({
            url: "/SqlApi/SearchText",
            async: true,
            data: { SearchString: searchText },
            type: 'POST',
            success: function (result) {
                try {
                   // debugger;
                    $("#elapsed_Time_sql").text(result.Performance.ElapsedTime);
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist_sql");
                    $("#count_sql").text(result.People.length);
                    $("#title_sql").text("Sql - SearchResult for Text: " + searchText);
                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                $("err_sql").text(ex.statusText)
            }
        });

        $.ajax({
            url: "/ElasticApi/SearchText",
            async: true,
            data: { SearchString: searchText },
            type: 'POST',
            success: function (result) {
                try {
                 //   debugger;
                    //$("#formatted_address").text(result.Formatted_Address);
                    $("#elapsed_Time_es").text(result.Performance.ElapsedTime);
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist_es");
                    $("#count_es").text(result.People.length);
                    $("#title_es").text("ElasticSearch - SearchResult for Text: " + searchText);
                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                $("err_es").text(ex.responseText);
            }
        });

    },

    GetAddress: function (lat, lng) {

        $("#peoplelist_sql").html('');
        $("#peoplelist_es").html('');
        $("#elapsed_Time_sql").text("na");
        $("#elapsed_Time_es").text("na");
        $("#count_sql").text("");
        $("#count_es").text("");

        $("#title_sql").text("Sql");
        $("#title_es").text("ElasticSearch");

        $.ajax({
            url: "/SqlApi/GetLocationDetail",
            async: true,
            data: { Lat: lat, Lng: lng },
            type: 'POST',
            success: function (result) {
                try {
                    $("#elapsed_Time_sql").text(result.Performance.ElapsedTime);
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist_sql");
                    $("#count_sql").text(result.People.length);
                    $("#title_sql").text("SQL:- " + result.Formatted_Address);
                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                $("err_sql").text(ex.responseText)
            }
        });

        $.ajax({
            url: "/ElasticApi/GetLocationDetail",
            async: true,
            data: { Lat: lat, Lng: lng },
            type: 'POST',
            success: function (result) {
                try {
                    $("#formatted_address").text(result.Formatted_Address);
                    $("#elapsed_Time_es").text(result.Performance.ElapsedTime);
                    $("#peoplelist-template").tmpl(result.People).appendTo("#peoplelist_es");
                    $("#count_es").text(result.People.length);
                    $("#title_es").text("ElasticSearch- : " + result.Formatted_Address);
                }
                catch (ex) {
                    alert('exception');
                }
            },
            error: function (ex) {
                $("err_es").text(ex.responseText)
            }
        });

    },
};