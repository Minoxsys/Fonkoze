$(function () {
    $('input.date').datepicker(
    { changeYear: true, changeMonth: true,
        onSelect: function(dateText, inst){               
                $(inst.input).trigger('datepickeronselect', dateText,inst);
            }
        }
        
    );

    $('#ui-datepicker-div').hide();
});
