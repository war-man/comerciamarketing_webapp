	/*  Wizard */
	jQuery(function ($) {
		"use strict";
		$('form#wrapped').attr('action', 'survey.php');
		$("#wizard_container").wizard({
			stepsWrapper: "#wrapped",
			submit: ".submit",
			beforeSelect: function (event, state) {
				if ($('input#website').val().length != 0) {
					return false;
				}
				if (!state.isMovingForward)
					return true;
				var inputs = $(this).wizard('state').step.find(':input');
				return !inputs.length || !!inputs.valid();
			}
		}).validate({
			errorPlacement: function (error, element) {
				if (element.is(':radio') || element.is(':checkbox')) {
					error.insertBefore(element.next());
				} else {
					error.insertAfter(element);
				}
			}
		});
		//  progress bar
		$("#progressbar").progressbar();
		$("#wizard_container").wizard({
			afterSelect: function (event, state) {
				$("#progressbar").progressbar("value", state.percentComplete);
				$("#location").text("(" + state.stepsComplete + "/" + state.stepsPossible + ")");
			}
		});
		// Validate select
		$('#wrapped').validate({
			ignore: [],
			rules: {
				select: {
					required: true
				}
			},
			errorPlacement: function (error, element) {
				if (element.is('select:hidden')) {
					error.insertAfter(element.next('.nice-select'));
				} else {
					error.insertAfter(element);
				}
			}
		});
	});

// Summary 
function getVals(formControl, controlType) {
	switch (controlType) {

		case 'question_1':
			// Get the value for a radio
			var value = $(formControl).val();
			$("#question_1").text(value);
			break;

		case 'question_2':
			// Get name for set of checkboxes
			var checkboxName = $(formControl).attr('name');

			// Get all checked checkboxes
			var value = [];
			$("input[name*='" + checkboxName + "']").each(function () {
				// Get all checked checboxes in an array
				if (jQuery(this).is(":checked")) {
					value.push($(this).val());
				}
			});
			$("#question_2").text(value.join(", "));
            break;

        case 'question_22':
			// Get name for set of checkboxes
			var checkboxName = $(formControl).attr('name');

			// Get all checked checkboxes
			var value = [];
			$("input[name*='" + checkboxName + "']").each(function () {
				// Get all checked checboxes in an array
				if (jQuery(this).is(":checked")) {
					value.push($(this).val());
				}
			});
			$("#question_22").text(value.join(", "));
			break;

		case 'question_3':
			// Get the value for a radio
			var value = $(formControl).val();
			$("#question_3").text(value);
            break;

        case 'question_4':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_4").text(value);
            break;
        case 'question_5':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_5").text(value);
            break;
        case 'question_6':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_6").text(value);
            break;
        case 'question_7':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_7").text(value);
            break;
        case 'question_3':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_3").text(value);
            break;
        case 'question_8':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_8").text(value);
            break;
        case 'question_9':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_9").text(value);
            break;
        case 'question_10':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_10").text(value);
            break;
        case 'question_11':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#question_11").text(value);
            break;

		case 'additional_message':
			// Get the value for a textarea
			var value = $(formControl).val();
			$("#additional_message").text(value);
            break;

        case 'additional_message':
            // Get the value for a textarea
            var value = $(formControl).val();
            $("#additional_message").text(value);
            break;

        case 'review_message':
            // Get the value for a textarea
            var value = $(formControl).val();
            $("#review_message").text(value);
            break;
        case 'review_message2':
            // Get the value for a textarea
            var value = $(formControl).val();
            $("#review_message2").text(value);
            break;
        case 'review_message3':
            // Get the value for a textarea
            var value = $(formControl).val();
            $("#review_message3").text(value);
            break;

        case 'favoritebeerbrand':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#favoritebeerbrand").text(value);
            break;

        case 'beerflavor':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#beerflavor").text(value);
            break;

        case 'flavortaste':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#flavortaste").text(value);
            break;

        case 'other':
            // Get the value for a radio
            var value = $(formControl).val();
            $("#other").text(value);
            break;


	}
}