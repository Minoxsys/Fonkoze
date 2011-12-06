/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="jquery-ui-1.8.14.min.js" />


$(function () {

    Date.prototype.addDays = function (days) {
        this.setDate(this.getDate() + days);
    }

    function transformStringDateToFormatMMDDYYYY(dateString) {

        var dateParts = dateString.split('/');
        return (dateParts[1] + '/' + dateParts[0] + '/' + dateParts[2]);
    }

    function calculateWorkingDays(startDate, endDate) {

        var wholeWeeks = 0;
        var extraDays = 0;
        var extraWorkingDays = 0;
        var workingWeek = 5;
        var weekDay = 0;
        var workDays = 0;
        var switchedDates = false;

        var dateTemp = new Date();

        var dateStart = new Date(transformStringDateToFormatMMDDYYYY(startDate));
        var dateEnd = new Date(transformStringDateToFormatMMDDYYYY(endDate));

        if (dateEnd < dateStart) {
            switchedDates = true;
            dateTemp = dateStart;
            dateStart = dateEnd;
            dateEnd = dateTemp;
        }
        else
            switchedDates = false;

        wholeWeeks = Math.floor((dateEnd.getTime() - dateStart.getTime()) / 604800000); // 1 week in milliseconds = 1000*60*60*24*7

        /* Calculate additional days outside in final part-week */
        extraDays = Math.floor((dateEnd.getTime() - dateStart.getTime()) / 86400000) % 7; // 1 day in milliseconds = 1000*60*60*24
        extraDays = extraDays + 1;
        /* Subtract if they are not working days */
        while (extraDays > 0) {


            weekDay = dateEnd.getDay();
            if (weekDay > 0 && weekDay < 6)
                extraWorkingDays = extraWorkingDays + 1;
            dateEnd.addDays(-1);
            extraDays = extraDays - 1;
        }

        workDays = (wholeWeeks * workingWeek) + extraWorkingDays;



        if (switchedDates == true) {
            workDays = Math.abs(workDays);
        }

        return workDays;
    }



    var update = function () {

        var txt1 = $("#ui-startDate").val();
        var txt2 = $("#ui-endDate").val();

        if (txt1 != "" && txt2 != "") {
            var startDate = txt1;
            var endDate = txt2;

            var d = calculateWorkingDays(startDate, endDate);
            $("#days").val(d);

        }
    };


    $("div.holidayDate").click(function () {
        var url = $(this).attr("url");
        window.location.href = url;
    });


    $("#ui-startDate").bind('datepickeronselect', function () { update(); });

    $("#ui-endDate").bind('datepickeronselect', function () { update(); });

    if ($("#ui-startDate").length) {
        update();
    }

});