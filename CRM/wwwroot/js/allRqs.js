function SetCurrentData(id) {
    console.log(id);
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
            $('#selectRespons').val(JSON.parse(json.users)).trigger('change.select2');
            var date = moment(json.last_changed);
            var formattedDate = date.format("DD MMMM YYYY HH:mm");
            $("#last_changed b").text(formattedDate);
            date = moment(json.open_date);
            formattedDate = date.format("DD MMMM YYYY HH:mm");
            $("#open_date b").text(formattedDate);
            var attachArray = JSON.parse(json.attach);
            $(".attachments div").empty();
            attachArray.forEach(function (value) {
                console.log(value);
                var link = $('<a></a>').appendTo($(".attachments div"));
                const fileName = value.split("\\").pop().split(".")[0]; 
                const fileExt = value.split(".").pop();
                link.attr('download', fileName);
                link.attr('href', value);
                $('<p></p>').attr('id', 'type').text(fileExt).appendTo(link);
                $('<p></p>').attr('id', 'file_name').text(fileName).appendTo(link);
            });
            $("#currentRequest").show();
        },
        error: function (xhr, status, error) {
            console.log('Request failed.  Returned status of ' + xhr.status);
        }
    });
}

$(document).ready(function () {
    setInterval(GetData(), 60000); // 60000 миллисекунд = 1 минута

    function GetData() {
        $.ajax({
            type: "GET",
            url: "AllRequests?handler=GetData",
            dataType: 'json',
            success: function (response) {
                var jsonArray = JSON.parse(response);
                var list = $("#ListSubj");
                list.empty();
                jsonArray.forEach(function (item) {
                    var li = $('<li></li>');
                        li.on('click', function () {
                            SetCurrentData(item.TicketId);
                        });
                        var button = $('<button></button>');
                        button.text(item.TicketTitle);
                        var table = $('<table></table>');
                        table.attr('width', '100%');
                        table.attr('class', "stage_" + item.StateNavigation.StateId);
                        var tr = $('<tr></tr>');
                        var p = $('<p></p>');
                        p.text(item.StateNavigation.Name);
                        for (var i = 0; i < 5; i++) {
                            var td = $('<td></td>');
                            tr.append(td);
                        }        
                        table.append(tr);
                        button.append(table);
                        button.append(p);
                        li.append(button);
                        li.append('<hr />');
                        list.append(li);
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    };

    moment.locale('ru');
    $(".history_button").click(function () {
        event.preventDefault(); 
        $(".history_block").toggleClass('invisible');
        $.ajax({
            type: "GET",
            url: "AllRequests?handler=GetHistory",
            dataType: 'text',
            success: function (response) {
                console.log(response);
                $(".history").html(response);
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    });

    $(document).mouseup(function (e) {
    var container = $(".history_block");
    if (container.has(e.target).length === 0 && container.hasClass("invisible") == false){
        $(".history_block").toggleClass('invisible');
    }
    });
    $("#add_comm").click(function () {
        event.preventDefault(); 
        var message = $("#commentRequest").val();
        var div = $("<div></div>");
        div.attr("class", "UserComm");
        var user = window.parent.$("#user_name").text();
        div.append("<p>" + user + "</p>" + message);
        $("#commentRequest").val("");
        $(".commentsHistory").append(div);
        //console.log(1);
        //$.ajax({
        //    type: "POST",
        //    url: "AllRequests?handler=PutComment",
        //    data: { message: message },
        //    success: function (response) {
               
        //    },
        //    error: function (xhr, status, error) {
        //        console.log('Request failed.  Returned status of ' + xhr.status);
        //    }
        //});
    });

    $('#submit').click( function (e) {
        e.preventDefault();
        var stage = $("#selectStage").val();
        var respons = $("#selectRespons").val();
        console.log(typeof respons);
        var formData = new FormData();
        formData.append('stage', stage);
        formData.append('respons', respons);
        console.log(formData);
        $.ajax({
            type: "POST",
            url: "AllRequests?handler=PutData",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                GetData();
                $(".item").hide();
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    });

   
});