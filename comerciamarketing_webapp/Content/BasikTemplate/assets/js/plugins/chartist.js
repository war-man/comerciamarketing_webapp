(function (global) {
	"use strict";
  
  	Chartist.plugins.animate = function(options) {
	  return function animate(chart) {
	    var defaultOptions = {
	      delay: 80,
	      duration: 500,
	      grid: false,
	      label: false
	    };

	    options = Chartist.extend({}, defaultOptions, options);

	    var seq = 0;
	    chart.on('created', function() {
		  seq = 0;
		});

	    chart.on('draw', function(data) {
		  if(data.type === 'label' && data.axis && data.axis.units.pos ==='x' && options.label) {
		  	seq++;
		    data.element.animate({
		      y: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: data.y + 100,
		        to: data.y,
		        easing: 'easeOutQuart'
		      }
		    });
		  } else if(data.type === 'label' && data.axis && data.axis.units.pos ==='y' && options.label) {
		  	seq++;
		    data.element.animate({
		      x: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: data.x - 100,
		        to: data.x,
		        easing: 'easeOutQuart'
		      }
		    });
		  }else if(data.type === 'grid' && options.grid) {
		  	seq++;
		    var pos1Animation = {
		      begin: seq * options.delay,
		      dur: options.duration,
		      from: data[data.axis.units.pos + '1'] - 30,
		      to: data[data.axis.units.pos + '1'],
		      easing: 'easeOutQuart'
		    };

		    var pos2Animation = {
		      begin: seq * options.delay,
		      dur: options.duration,
		      from: data[data.axis.units.pos + '2'] - 100,
		      to: data[data.axis.units.pos + '2'],
		      easing: 'easeOutQuart'
		    };

		    var animations = {};
		    animations[data.axis.units.pos + '1'] = pos1Animation;
		    animations[data.axis.units.pos + '2'] = pos2Animation;
		    animations['opacity'] = {
		      begin: seq * options.delay,
		      dur: options.duration,
		      from: 0,
		      to: 1,
		      easing: 'easeOutQuart'
		    };

		    data.element.animate(animations);
		  } else if(data.type === 'point') {
		  	seq++;
		    data.element.animate({
		      x1: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: data.x - 10,
		        to: data.x,
		        easing: 'easeOutQuart'
		      },
		      x2: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: data.x - 10,
		        to: data.x,
		        easing: 'easeOutQuart'
		      },
		      opacity: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: 0,
		        to: 1,
		        easing: 'easeOutQuart'
		      }
		    });
		  } else if(data.type === 'line') {
		  	seq++;
		    data.element.animate({
		      opacity: {
		        begin: seq * options.delay + 1000,
		        dur: options.duration,
		        from: 0,
		        to: 1
		      }
		    });
		  } else if(data.type === 'area') {
		  	seq++;
		  	data.element.animate({
		      d: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: data.path.clone().scale(1, 0).translate(0, data.chartRect.height()).stringify(),
		        to: data.path.clone().stringify(),
		        easing: Chartist.Svg.Easing.easeOutQuint
		      }
		    });
		  }else if(data.type === 'bar') {
		  	seq++;
		  	data.element.animate({
		      opacity: {
		        begin: seq * options.delay,
		        dur: options.duration,
		        from: 0,
		        to: 1,
		        easing: 'easeOutQuart'
		      }
		    });
		  }else if(data.type === 'slice') {
		  	seq++;
		  	var pathLength = data.element._node.getTotalLength();
		    data.element.attr({
		      'stroke-dasharray': pathLength + 'px ' + pathLength + 'px'
		    });
		    var animationDefinition = {
		      'stroke-dashoffset': {
		        id: 'anim' + data.index,
		        dur: 1000,
		        from: -pathLength + 'px',
		        to:  '0px',
		        easing: Chartist.Svg.Easing.easeOutQuint,
		        fill: 'freeze'
		      }
		    };
		    if(data.index !== 0) {
		      animationDefinition['stroke-dashoffset'].begin = 'anim' + (data.index - 1) + '.end';
		    }
		    data.element.attr({
		      'stroke-dashoffset': -pathLength + 'px'
		    });
		    data.element.animate(animationDefinition, false);
		  }
		});
	  }
	}

  var init = function(){
    new Chartist.Line('#chartist-line', {
	  labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
	  series: [
	  	{value: [12, 9, 7, 8, 5], className: 'ct-series-a ct-stroke-3', meta: 'Google'},
	  	{value: [3, 1.5, 3.5, 6, 3], className: 'ct-series-e ct-stroke-4', meta: 'Facebook'},
	  	{value: [1, 3, 4, 5, 6], className: 'ct-series-h ct-stroke-5', meta: 'Twitter'}
	  ]
	}, {
	  fullWidth: true,
	  chartPadding: {
	    right: 40
	  },
	  plugins: [
	    Chartist.plugins.tooltip(),
	    Chartist.plugins.animate({grid: true, label: true})
	  ]
	});

	new Chartist.Line('#chartist-line-area', {
	  labels: [1, 2, 3, 4, 5, 6, 7, 8],
	  series: [
	    {value: [1, 2, 3, 1, -2, 0, 1, 0], className: 'ct-series-a', meta: 'Google'},
	    {value: [-2, -1, -2, -1, -2.5, -1, -2, -1], className: 'ct-series-b', meta: 'Apple'},
	    {value: [0, 0, 0, 1, 2, 2.5, 2, 1], className: 'ct-series-c', meta: 'Microsoft'},
	    {value: [2.5, 2, 1, 0.5, 1, 0.5, -1, -2.5], className: 'ct-series-h', meta: 'Tesla'}
	  ]
	}, {
  	  showArea: true,
  	  showPoint: true,
  	  showLine: false,
  	  fullWidth: true,
	  axisX: {
	    showLabel: false,
	    showGrid: false
	  },
  	  lineSmooth: Chartist.Interpolation.simple({
	    divisor: 2
	  }),
	  plugins: [
	  	Chartist.plugins.tooltip(),
	    Chartist.plugins.animate()
	  ]
	});

	new Chartist.Bar('#chartist-bar', {
		labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
		series: [
		    {value: [5, 4, 3, 7, 5, 10, 3, 4, 8, 10, 6, 8], className: 'ct-series-a ct-stroke-4', meta: 'Facebook'},
		    {value: [3, 2, 9, 5, 4, 6, 4, 6, 7, 8, 7, 4], className: 'ct-series-g ct-stroke-4', meta: 'Twitter'}
		]
	},{
		seriesBarDistance: 8,
		plugins: [
		    Chartist.plugins.tooltip(),
	    	Chartist.plugins.animate()
		]
	});

	new Chartist.Bar('#chartist-h-bar', {
	  labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
	  series: [
	    {value: [5, 4, 3, 7, 5, 10, 3], className: 'ct-series-c ct-stroke-3', meta: 'Google'},
	    {value: [3, 2, 9, 5, 4, 6, 4], className: 'ct-series-h ct-stroke-3', meta: 'Bing'}
	  ]
	}, {
	  seriesBarDistance: 6,
	  horizontalBars: true,
	  axisY: {
	    offset: 70
	  },
	  plugins: [
	    Chartist.plugins.tooltip(),
	    Chartist.plugins.animate()
	  ]
	});

	new Chartist.Pie('#chartist-pie', {
		labels: ['Bananas', 'Apples', 'Grapes'],
		  series: [{
		     value: 4,
		     className: 'ct-series-a'
		   }, {
		     value: 3,
		     className: 'ct-series-b'
		   },{
		     value: 5,
		     className: 'ct-series-g'
		   }]
		}
	  , {
	  donut: true,
	  donutWidth: 80,
	  chartPadding: 40,
	  labelOffset: 60,
	  labelDirection: 'explode',
	  labelInterpolationFnc: function(value) {
	      return value;
	  },
	  plugins: [
	    Chartist.plugins.tooltip(),
	    Chartist.plugins.animate()
	  ]
	});

	new Chartist.Pie('#chartist-dougnut', {
	  series: [{
	     value: 20,
	     className: 'ct-series-k',
	     meta: 'Apples'
	   }, {
	     value: 10,
	     className: 'ct-series-l',
	     meta: 'Grapes'
	   }, {
	     value: 70,
	     className: 'ct-series-g',
	     meta: 'Bananas'
	   }]
	}, {
	  donut: true,
	  donutWidth: 20,
	  startAngle: 270,
	  showLabel: true,
	  chartPadding: 45,
	  labelOffset: 30,
   	  labelDirection: 'explode',
	  plugins: [
	    Chartist.plugins.tooltip(),
	    Chartist.plugins.animate()
	  ]
	});

  }

  global.chartist = {init: init};

})(this);
