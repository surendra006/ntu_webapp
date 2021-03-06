/**
 * Automatically detect numbers which use a comma in the place of a decimal 
 * point to allow them to be sorted numerically.
 * 
 * Please note that the 'Formatted numbers' type detection and sorting plug-ins
 * offer greater flexibility that this plug-in and should be used in preference
 * to this method.
 *  @name Commas for decimal place
 *  @anchor numeric_comma
 *  @author <a href="http://sprymedia.co.uk">Allan Jardine</a>
 */

jQuery.fn.dataTableExt.aTypes.unshift(
	function ( sData )
	{
		var sValidChars = "0123456789,.";
		var Char;
		var bDecimal = false;
		var iStart=0;

		/* Negative sign is valid - shift the number check start point */
		if ( sData.charAt(0) === '-' ) {
			iStart = 1;
		}

		/* Check the numeric part */
		for ( i=iStart ; i<sData.length ; i++ )
		{
			Char = sData.charAt(i);
			if (sValidChars.indexOf(Char) == -1)
			{
				return null;
			}
		}

		return 'numeric-comma';
	}
);

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "numeric-comma-pre": function (a) {
        var x = (a == "-") ? 0 : a.replace(/,/, ".");
        return parseFloat(x);
    },

    "numeric-comma-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "numeric-comma-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});