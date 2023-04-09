$(document).ready(function () {    
    
    $('.main_block li').each(function (index) {
      if (index % 3 === 0) {
        $(this).on('click', function () {
          if($(".changed_panel").children().length != 0){
            $(".changed_panel").empty();
          }
          const iframe = document.createElement('iframe');
          iframe.src = 'AllRequests';
          iframe.height = '100%';
          iframe.width = '100%';
          $(iframe).css("border", "none");
          $(".changed_panel").append(iframe);
        });
      }
    });

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