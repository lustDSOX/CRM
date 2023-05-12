function SetCurrentData(elem) {
    var id = $(elem).attr("id");
    $.ajax({
        type: "GET",
        url: "AllRequests?handler=SetData&id=" + id,
        dataType: 'json',
        success: function (response) {
            var json = response;
            $('#currentRequest h4').text(json.title);
            $("#desciption").html(json.desciption);
            $("#requester").text(json.requester);
            $('#selectStage').val(json.state).trigger('change.select2');
            $('#selectRespons').val(json.users).trigger('change.select2');
            var date = moment(json.last_changed);
            var formattedDate = date.format("DD MMMM YYYY HH:mm");
            $("#last_changed b").text(formattedDate);
            date = moment(json.open_date);
            formattedDate = date.format("DD MMMM YYYY HH:mm");
            $("#open_date b").text(formattedDate);
            $("#currentRequest").show();
        },
        error: function (xhr, status, error) {
            console.log('Request failed.  Returned status of ' + xhr.status);
        }
    });
}

$(document).ready(function () {
    
    moment.locale('ru');
    $( ".history_button" ).click(function() {
        $(".history_block").toggleClass('invisible');
    });

    $(document).mouseup(function (e) {
    var container = $(".history_block");
    if (container.has(e.target).length === 0 && container.hasClass("invisible") == false){
        $(".history_block").toggleClass('invisible');
    }
    });

   
});