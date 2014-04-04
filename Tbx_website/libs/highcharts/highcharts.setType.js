(function(Highcharts) {
    var each = Highcharts.each,
        baseInit = Highcharts.Series.prototype.init;
    
    Highcharts.Series.prototype.init = function(chart, options) {
        this.ownOptions = options;
        this.colorIdx = chart.counters.color;
        this.symbolIdx = chart.counters.symbol;
        baseInit.call(this, chart, options);
    };
        
        
    
    Highcharts.Series.prototype.setType = function(type) {
        var series = this,
            i, 
            chart = this.chart,
            options = this.ownOptions;
        
        // Set back color
        chart.counters.color = this.colorIdx;
        chart.counters.symbol = this.symbolIdx;
        options.type = type;
        options.animation = false;
        options.visible = this.visible; // why?
        
        
        //--- The following is a subset of series.destroy(), so a core
        // implementation should re-use that
        
        var data = this.data,
            seriesClipRect = series.clipRect,
            point;
        // destroy all points with their elements
        i = data.length;
        while (i--) {
            point = data[i];
            if (point && point.destroy) {
                point.destroy();
            }
        }
        series.points = null;

        // If this series clipRect is not the global one we
        // destroy it here.
        if (seriesClipRect && seriesClipRect !== chart.clipRect) {
            series.clipRect = seriesClipRect.destroy();
        }

        // destroy all SVGElements associated to the series
        each(['area', 'graph', 'dataLabelsGroup', 'group', 'tracker'], function (prop) {
            if (series[prop]) {
                series[prop] = series[prop].destroy();
            }
        });
        //--- End subset
        
        
        // Remove current prototype methods and add new
        for (i in this) {
            if (typeof this[i] === 'function' && this.hasOwnProperty(i)) {
                delete this[i];
            }
        }

        Highcharts.extend(this, Highcharts.seriesTypes[type].prototype);
        
        this.init(chart, options);
        
        
        chart.redraw();
    };
    
})(Highcharts);