$(document).ready(function () {    
    
  var elements = $('.main_block li');
  elements.on('click', function() {
    var index = elements.index(this);
    GetReq(index);
  });


  function GetReq(i){
    if($(".changed_panel").children().length != 0){
            $(".changed_panel").empty();
          }
    const iframe = document.createElement('iframe');
    console.log(i);
    switch(i){
      case 0:
        iframe.src = 'AllRequests?id=-1';
        break;
      case 1:
        iframe.src = 'AllRequests';    
        break;
      case 2:
        iframe.src = 'Listeners';
        break;
      case 3:
        iframe.src = 'Requesters';
        break;
      case 4:
        iframe.src = 'Users';
        break;
    }
    iframe.height = '100%';
    iframe.width = '100%';
    $(iframe).css("border", "none");
    $(".changed_panel").append(iframe);
  }

	$( ".user_button" ).click(function() {
		$(".user_panel").toggleClass('invisible');
	});

	$(document).mouseup(function (e) {
    var container = $(".user_panel");
    if (container.has(e.target).length === 0 && container.hasClass("invisible") == false){
        $(".user_panel").toggleClass('invisible');
    }
    });

});