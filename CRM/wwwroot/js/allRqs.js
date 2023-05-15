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
            var attachArray = JSON.parse(json.attach);
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
    GetData();

    function GetData() {
        $.ajax({
            type: "GET",
            url: "AllRequests?handler=GetData",
            dataType: 'json',
            success: function (response) {
                var jsonArray = JSON.parse(response);
                var list = $("#ListSubj");
                jsonArray.forEach(function (item) {
                    if (list.find("#" + item.TicketId).length != 0) {
                        var li = list.find("#" + item.TicketId);
                        var table = li.find('table');
                        var button = li.find('button');
                        var p = li.find('p');
                        table.attr('class', "stage_" + item.StateNavigation.StateId);
                        button.text(item.TicketTitle);
                        p.text(item.StateNavigation.Name);
                    }
                    else {
                        var li = $('<li></li>');
                        li.on('click', SetCurrentData.bind(this, li));
                        li.attr('id', item.TicketId );
                        var button = $('<button></button>');
                        var table = $('<table></table>');
                        table.attr('width', '100%');
                        var tr = $('<tr></tr>');
                        for (var i = 0; i < 5; i++) {
                            var td = $('<td></td>');
                            tr.append(td);
                        }
                        table.append(tr);
                        var p = $('<p></p>');
                        table.attr('class', "stage_" + item.StateNavigation.StateId);
                        button.text(item.TicketTitle);
                        p.text(item.StateNavigation.Name);
                        button.append(table);
                        button.append(p);
                        li.append(button);
                        li.append('<hr />');
                        list.append(li);
                    }
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    };

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

    $('form').on('submit', function (e) {
        e.preventDefault();
        var stage = $("#selectStage").val();
        var respons = $("#selectRespons").val();

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

    $(".option_btns button:first").on("click", function () {
        var message = $("#commentRequest").val();
            $.ajax({
                type: "POST",
                url: "AllRequests?handler=PutComment&message=" + message,
                success: function (response) {
                    $("#commentRequest").val("");
                },
                error: function (xhr, status, error) {
                    console.log('Request failed.  Returned status of ' + xhr.status);
                }
            });
    });
});