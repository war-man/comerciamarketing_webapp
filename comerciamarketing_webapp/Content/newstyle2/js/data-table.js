(function($) {
  'use strict';
  $(function() {
      $('#order-listing').DataTable({
          dom: 'lfrtiBp',
     
      "aLengthMenu": [
        [5, 10, 15, -1],
        [5, 10, 15, "All"]
          ],
          buttons: [
              'excelHtml5',
              'pdfHtml5'
          ],
      "iDisplayLength": 10,
      "language": {
        search: ""
      }
    });
    $('#order-listing').each(function() {
      var datatable = $(this);
      // SEARCH - Add the placeholder for Search and Turn this into in-line form control
      var search_input = datatable.closest('.dataTables_wrapper').find('div[id$=_filter] input');
      search_input.attr('placeholder', 'Search');
      search_input.removeClass('form-control-sm');
      // LENGTH - Inline-Form control
      var length_sel = datatable.closest('.dataTables_wrapper').find('div[id$=_length] select');
      length_sel.removeClass('form-control-sm');
    });
  });
})(jQuery);

(function ($) {
    'use strict';
    $(function () {
        $('#retailaudit').DataTable({
            dom: '',
            ordering: false,
            paging: false,
            responsive: true
     
        });
    });
})(jQuery);

(function ($) {
    'use strict';
    $(function () {
        $('#order-listing2').DataTable({
            dom: 'lfrtiBp',

            "aLengthMenu": [
                [5, 10, 15, -1],
                [5, 10, 15, "All"]
            ],
            buttons: [
                'excelHtml5',
                'pdfHtml5'
            ],
            "iDisplayLength": 10,
            "language": {
                search: ""
            },
            "columnDefs": [{
                "targets": 2,    // column index, 0 means the first column
                "render": function (data) {
                    return moment(data, 'MM/DD/YY').format('MM/DD/YY');
                }
            }],
            "order": [[1, "asc"]]
        });
        $('#order-listing2').each(function () {
            var datatable = $(this);
            // SEARCH - Add the placeholder for Search and Turn this into in-line form control
            var search_input = datatable.closest('.dataTables_wrapper').find('div[id$=_filter] input');
            search_input.attr('placeholder', 'Search');
            search_input.removeClass('form-control-sm');
            // LENGTH - Inline-Form control
            var length_sel = datatable.closest('.dataTables_wrapper').find('div[id$=_length] select');
            length_sel.removeClass('form-control-sm');
        });
    });
})(jQuery);