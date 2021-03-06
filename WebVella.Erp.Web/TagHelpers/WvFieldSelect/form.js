﻿
function SelectFormGenerateSelectors(fieldId, fieldName, config) {
	//Method for generating selector strings of some of the presentation elements
	var selectors = {};
	if (!config.prefix || config === "") {
		selectors.inputEl = "#input-" + fieldId;
	}
	else {
		selectors.inputEl = "#input-" + config.prefix + "-" + fieldId;
	}
	selectors.modalEl = "#add-option-modal-" + fieldId;
	selectors.primaryBtnEl = "#add-option-modal-" + fieldId + " .btn-primary";
	selectors.modalFormEl = "#add-option-form-" + fieldId;
	return selectors;
}

function SelectFormFormat(icon) {
	var originalOption = icon.element;
	var iconClass = $(originalOption).data('icon');
	var color = $(originalOption).data('color');
	if (!iconClass) {
		return icon.text;
	}
	return '<i class="fa ' + iconClass + '" style="color:' + color + '"></i> ' + icon.text;
}

function SelectFormInit(fieldId, fieldName, entityName, config) {
	config = ProcessConfig(config);
	var selectors = SelectFormGenerateSelectors(fieldId, fieldName, config);

	$(selectors.inputEl).select2({
		language: "en",
		minimumResultsForSearch: 10,
		closeOnSelect: true,
		placeholder: 'not selected',
		allowClear:!$(selectors.inputEl).prop('required'),
		width: 'style',
		escapeMarkup: function (markup) {
			return markup;
		},
		templateResult: SelectFormFormat,
		templateSelection: SelectFormFormat
	}).on("select2:unselecting", function (e) {
		$(this).data('state', 'unselected');
	}).on("select2:open", function (e) {
		if ($(this).data('state') === 'unselected') {
			$(this).removeData('state');
			var self = $(this);
			setTimeout(function () {
				self.select2('close');
			}, 1);
		}
	});
	if (config.is_invalid) {
		$(selectors.inputEl).closest(".input-group").find(".select2-selection").addClass("is-invalid");
	}

	//COnfig-a dolu si stoi na posledno otvorenia realno a ne na tozi kum koito purvonachalno e bil initnat tozi event. Stranno ..
	$(selectors.inputEl).on('select2:open', function () {
		var appendLinkString = "<a href=\"javascript:void(0)\" onclick=\"addOptionModal('" + fieldId + "','" + fieldName + "','" + entityName + "','" + config.prefix + "')\" class=\"select2-add-option\"><i class=\"fa fa-plus-circle\"></i> create new record</a>";
		if (config && config.can_add_values) {
			$(".select2-results:not(:has(a))").append(appendLinkString);
		}
	});

	$(selectors.inputEl).on('change', function (event) {
		var customEvent = new Event('WvFieldSelect_Change');
		var inputElement = document.getElementById('input-' + fieldId);
		var selectedJson = $(selectors.inputEl).select2('data');
		var selectedKey = null;
		if (selectedJson.length > 0) {
			selectedKey = selectedJson[0].id; //this is a single select
		}
		customEvent.payload = {
			value: selectedKey,
			fieldId: fieldId,
			fieldName: inputElement.name
		};
		document.dispatchEvent(customEvent);
	});

	$(selectors.modalEl).appendTo("body");
	var $form = $('<form id="add-option-modal-' + fieldId + '" name="add-option-modal-' + fieldId + '"></form>');
	$(selectors.modalEl + " .modal-dialog").append($form);
	$(selectors.modalEl + " .modal-content").appendTo(selectors.modalEl + " form");
	$(selectors.modalEl + " form").on("submit", function (event) {
		event.preventDefault();
		var $alertMessage = $(selectors.modalEl).find(".alert-danger");
		$($alertMessage).addClass("d-none").html("");
		var inputValue = $(selectors.modalEl).find(".add-option-input").val();
		if (!isStringNullOrEmptyOrWhiteSpace(inputValue)) {
			var submitObj = {};
			submitObj.value = inputValue;
			submitObj.entityName = entityName;
			submitObj.fieldName = fieldName;
			$(selectors.modalEl).find(".btn-primary").attr("disabled", "disabled").find(".fa").removeClass("fa-plus-circle").addClass("fa-spin fa-spinner");
			$.ajax({
				headers: {
					'Accept': 'application/json',
					'Content-Type': 'application/json'
				},
				type: "PUT",
				url: '/api/v3.0/p/core/select-field-add-option',
				data: JSON.stringify(submitObj),
				success: function (response) {
					if (response.success) {
						addOptionSuccessCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
					}
					else {
						addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
					}
				},
				error: function (jqXHR, textStatus, errorThrown) {
					var response = {};
					response.message = "";
					if (jqXHR && jqXHR.responseJSON) {
						response.message = jqXHR.responseJSON.message;
					}
					addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, config.prefix);
				}
			});
		}
		else {
			$($alertMessage).html("Required field").removeClass("d-none");
		}
	});
}

function addOptionSuccessCallback(response, fieldId, fieldName, entityName, inputValue, prefix) {
	var selectorInputEl = "";

	if (!prefix || prefix === "") {
		selectorInputEl = "#input-" + fieldId;
	}
	else {
		selectorInputEl = "#input-" + prefix + "-" + fieldId;
	}
	var selectorModalEl = "#add-option-modal-" + fieldId;
	var newOption = new Option(inputValue, inputValue, false, false);
	$(selectorInputEl).append(newOption);
	$(selectorInputEl).select2().val(inputValue).trigger('change');
	$(selectorModalEl).modal('hide');
}

function addOptionErrorCallback(response, fieldId, fieldName, entityName, inputValue, prefix) {
	var selectorInputEl = "";

	if (!prefix || prefix === "") {
		selectorInputEl = "#input-" + fieldId;
	}
	else {
		selectorInputEl = "#input-" + prefix + "-" + fieldId;
	}
	var selectorModalEl = "#add-option-modal-" + fieldId;
	var $alertMessage = $(selectorModalEl).find(".alert-danger");
	$(selectorModalEl).find(".btn-primary").removeAttr("disabled", "disabled").find(".fa").addClass("fa-plus-circle").removeClass("fa-spin fa-spinner");
	$($alertMessage).html(response.message).removeClass("d-none");
}

function addOptionModal(fieldId, fieldName, entityName, prefix) {
	var selectorInputEl = "";

	if (!prefix || prefix === "") {
		selectorInputEl = "#input-" + fieldId;
	}
	else {
		selectorInputEl = "#input-" + prefix + "-" + fieldId;
	}


	var selectorModalEl = "#add-option-modal-" + fieldId;
	var $alertMessage = $(selectorModalEl).find(".alert-danger");
	$($alertMessage).addClass("d-none").html("");

	$(selectorModalEl).on('shown.bs.modal', function () {
		$(selectorModalEl).find(".add-option-input").val("");
		$(selectorModalEl).find(".btn-primary").removeAttr("disabled", "disabled").find(".fa").addClass("fa-plus-circle").removeClass("fa-spin fa-spinner");
		$(selectorInputEl).select2("close");
		$('.add-option-input').trigger('focus');
	});

	$(selectorModalEl).modal();
}
