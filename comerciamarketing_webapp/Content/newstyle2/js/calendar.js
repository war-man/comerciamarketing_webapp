(function($) {
  'use strict';
  $(function() {
      var style = getComputedStyle(document.body);
      var today = new Date();
      today.setHours(0, 0, 0, 0);

    if ($('#calendar').length) {
      $('#calendar').fullCalendar({
        header: {
          left: 'prev, next, today',
          center: 'title',
          right: 'month,listWeek,listDay'
          },
          views: {
              month: { buttonText: 'Month' },
              listWeek: { buttonText: 'list week' },
              listDay: { buttonText: 'list day' }
          },

        defaultDate: today,
        navLinks: true, // can click day/week names to navigate views
          editable: true,
          //showNonCurrentDates: false,
        eventLimit: true, // allow "more" link when too many events
        events: [{
            title: 'All Day Event',
            start: '2018-08-16'
          },
          {
            title: 'Long Event',
            start: '2018-08-09',
            end: '2018-08-10',
            color: style.getPropertyValue('--info')
            },
            {
                title: 'Long Event',
                start: '2018-08-09',
                end: '2018-08-10',
                color: style.getPropertyValue('--info')
            },
            {
                title: 'Long Event',
                start: '2018-08-09',
                end: '2018-08-10',
                color: style.getPropertyValue('--info')
            },
          {
            id: 999,
            title: 'Repeating Event',
            start: '2018-08-09T16:00:00',
            color: style.getPropertyValue('--danger')
          },
          {
            id: 999,
            title: 'Repeating Event',
            start: '2018-08-16T16:00:00',
            color: style.getPropertyValue('--info')
          },
          {
            title: 'Conference',
            start: '2018-08-11',
            end: '2018-08-13'
          },
          {
            title: 'Meeting',
            start: '2018-08-12T10:30:00',
            end: '2018-08-12T12:30:00',
            color: style.getPropertyValue('--danger')
          },
          {
            title: 'Click link',
            url: 'http://google.com/',
            start: '2018-08-28',
            color: style.getPropertyValue('--danger')
          }
        ]
      })
    }
  });
})(jQuery);