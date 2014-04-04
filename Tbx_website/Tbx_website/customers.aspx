<!DOCTYPE html>
<html>
<head>
<!-- Fonts -->
	<link href="./fonts/fontAwesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" media="screen" title="stylesheet" />
	<link href="./fonts/fontello/css/fontello.css" rel="stylesheet" type="text/css" media="screen" title="stylesheet" />
	<link href='http://fonts.googleapis.com/css?family=PT+Sans:400,700,400italic,700italic' rel='stylesheet' type='text/css' />
	
	<!-- CSS -->
	<link href="./css/stylenew.css" rel="stylesheet" type="text/css" media="screen" title="stylesheet" />
	<link href="./css/responsive1.css" rel="stylesheet" type="text/css" media="screen" title="stylesheet" />
	<link href="./css/custom1.css" rel="stylesheet" type="text/css" media="screen" title="stylesheet" />

    <!-- Scripts -->
	<script src="/libs/jquery-ui-1.10.1.custom.min.js"></script>
    <script src="libs/jquery-1.8.2.min.js" type="text/javascript"></script>
    </head>

    <body>
    <div class="full-block">
    <div class="customer-portal-heading">Customer Portal</div>
    <div class="welcome-message"> Welcome "Customer-name" </div>

    <div class="customer-portal">     
    
			<ul class="inputs black-input large">
				<!-- The autocomplete="off" attributes is the only way to prevent webkit browsers from filling the inputs with yellow -->
                <li><span class="icon-plus mid-margin-right" id="plus-minus"></span><a class="my-projects">My Projects</a></li>       
                <div class="list-circle">        
                <ul id="My-projects-hide"> 
                <li>Crowd-simulation API </li>
                <li>Interactive game design API </li>
                 </ul>
                 </div>
                <li><span class="icon-download  mid-margin-right"></span><a class="Available-products" href="Available_Projects_list_for_customers.aspx">Available Projects</a></li>                
				<li><span class=" icon-question-sign mid-margin-right"></span><a class="active enquiry">Product Enquiry</a></li>   
                <div class="textarea-button-combo"><textarea rows="5" cols="26" id="prod-desc" type="text" placeholder="Couldnt find the project you are looking for ?? Enter the description of the project you are looking for !!"></textarea>             
                 <div class="enquiry-button"><button type="submit" class="button glossy full-width huge">Submit Enquiry</button></div></div>
                </div>                
			</ul>
          </div>
		</div>

        <script>
            $(document).ready(function () {
                $('#My-projects-hide').hide();
                $('.textarea-button-combo').show();
            });      

            $(".my-projects").click(function () {
                if ($('#plus-minus').hasClass("icon-plus"))
            {
                $('#My-projects-hide').show();
                $('#plus-minus').removeClass("icon-plus");
                $('#plus-minus').addClass("icon-minus");            sdfafas
             }
            else {
                $('#My-projects-hide').hide();
                $('#plus-minus').removeClass("icon-minus");
                $('#plus-minus').addClass("icon-plus");
            }
        });
        var name = "Surendran";
        $('.enquiry-button').click(function () {
            $.ajax({
                type: "get",
                url: "http://localhost:49820/session/new/" + " " + name
                //			url: window.WebApiURL + token + "/settings/"+ $(this).val() + "/"+maxval1+"/"
            }).done(function (dataset) {
                console.debug('success from customers');
                alert("Dear" + dataset + "," + " " + "Your Query has been submitted. Please wait for our response");
            });
        });
  
        
        $(".enquiry").click(function () {
            if ($('.textarea-button-combo').hasClass("inactive")) {
                $('.textarea-button-combo').show();
                $('.textarea-button-combo').removeClass("inactive");
            }
            else {
                $('.textarea-button-combo').hide();
                $('.textarea-button-combo').addClass("inactive");
            }
        });         

        </script>
        </body>
        </html>
 
