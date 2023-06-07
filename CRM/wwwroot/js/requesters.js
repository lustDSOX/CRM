$(document).ready(function () {
    GetData();

    function handleDoubleClick(elementId) {
        parent.SetRqs(elementId);
    }

    function SetCurrentData() {
        var id = $(this).find('button').attr("id");
        $.ajax({
            type: "GET",
            url: "Requesters?handler=SetData&id=" + id,
            dataType: 'text',
            success: function (response) {
                var data = JSON.parse(response);
                var json = JSON.parse(data);
               /* console.log(json);*/
                $("#current").show();
                $('#email').val(json.Requester.Email);
                $("#number").val(json.Requester.PhoneNumber);
                var x = 0;
                $('tbody').empty();
                json.Tickets.forEach(function (item) {
                    var tr = $("<tr></tr>");
                    var td_name = $("<td></td>");
                    var td_state = $("<td></td>");
                    var td_users = $("<td></td>");
                    var td_date = $("<td></td>");

                    td_name.text(item.TicketTitle);
                    var date = moment(json.OpenDate);
                    var formattedDate = date.format("DD MMMM YYYY HH:mm");
                    td_date.text(formattedDate);
                    td_state.text(item.StateNavigation.Name);
                    var string_us = "";
                    if (json.UFT[x].length == 0) {
                        string_us = "-";
                    }
                    else {
                        for (var i = 0; i < json.UFT[x].length; i++) {
                            string_us += json.UFT[x][i] + '\n';
                        }
                    }
                    td_users.text(string_us);

                    tr.append(td_name);
                    tr.append(td_state);
                    tr.append(td_users);
                    tr.append(td_date);
                    tr.attr("id", item.TicketId);
                    tr.dblclick(function () {
                        var elementId = $(this).attr('id');
                        handleDoubleClick(elementId);
                    });
                    $('tbody').append(tr);
                    x+=1;
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    }

    function GetData() {
        $.ajax({
            type: "GET",
            url: "Requesters?handler=GetData",
            dataType: 'text',
            success: function (response) {
                var jsonArray = JSON.parse(response);
               /* console.log(jsonArray);*/
                var list = $("#ListSubj");
                jsonArray.forEach(function (item) {
                    if (list.find("#" + item.reqId).length != 0) {
                        var span = $("#" + item.reqId).siblings().filter("span");
                    }
                    else {
                        var li = $("<li></li>");
                        li.on('click', SetCurrentData);
                        var button = $("<button></button>");
                        var span = $("<span></span>");
                        button.attr("id", item.reqId);
                        button.append(span);
                        li.append(button);
                        list.append(li);
                    }
                    var hr = $("<hr/>");
                    li.append(hr);
                    span.text(item.email);
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        })
    };

    $('form').on('submit', function (e) {
        e.preventDefault();
        var number = $("#number").val();
        $.ajax({
            type: "POST",
            url: "Requesters?handler=PutData&number=" + number,
            success: function (response) {
                GetData();
                $("#current").hide();
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    });
});
