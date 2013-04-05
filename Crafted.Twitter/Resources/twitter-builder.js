

//get a reference to the script tag that contains this script, this could be an issue if it is present more than once
var scripts = document.getElementsByTagName('script');
var script = scripts[scripts.length - 1];

var inputParams = script.getAttribute('data-params');

jQuery.ajax({
    url: '/TwitterHandler/TimelineService',
    data: inputParams,
    data: { inputParameters: inputParams },
    dataType: 'json',
    type: 'GET',
    contentType: "application/json; charset=utf-8",
    success: BuildTweetStructure,
    complete: function () {
        //alert('done');
    },
    error: function () {
        jQuery(script).parent().append("<p class='service-error'>Unable to contact twitter service.</p>");
    }
});

function BuildTweetStructure(data) {

    var list = jQuery('<ul>');

    jQuery(data).each(function () {
        jQuery(list).append(BuildTweet(this));
    });

    jQuery(script).parent().append(list);
}

function BuildTweet(tweetData) {

    var tweetMap = FormatJsonString(tweetData, tweetLayout);

    var tweetItem = jQuery(tweetMap);

    return tweetItem;
}

// Formats a json object into a string using the supplied format string
// Json placeholders must be the property name wrapped in braces ie, {property}
// Nested properties to 3 descendants can be used as follows {Property.SubProperty.Data}
function FormatJsonString(jsonData, formatString) {

    var outputString = formatString;

    outputString = outputString.replace(/(\{)([\w\d-]+)(:[\w\d]+)?(\})/g, function (match, p1, p2, p3) { return FormatData(jsonData[p2], p3); });
    outputString = outputString.replace(/(\{)([\w\d-]+)(\.)([\w\d-]+)(:[\w\d]+)?(\})/g, function (match, p1, p2, p3, p4, p5) { return jsonData[p2][p4]; });
    outputString = outputString.replace(/(\{)([\w\d-]+)(\.)([\w\d-]+)(\.)([\w\d-]+)(:[\w\d]+)?(\})/g, function (match, p1, p2, p3, p4, p5, p6, p7) { return jsonData[p2][p4][p6]; });

    return outputString;
}

function FormatData(data, formatType) {

    switch (formatType) {
        case ':Date':
            var dateTimestamp = data.replace(/\D/g, '') * 1;
            var castDate = new Date(dateTimestamp);
            var dateString = castDate.getFullYear() + '/' + castDate.getMonth() + '/' + castDate.getDate() + ' ' + castDate.getHours() + ':' + castDate.getMinutes() + ':' + castDate.getSeconds();
            return dateString;
            break;
        case ':TimePassed':
            var dateTimestamp = data.replace(/\D/g, '') * 1;
            var castDate = new Date(dateTimestamp);
            return timeSince(castDate);
            break;
        case ':RichText':
            return ParseRichText(data);
            break;
        default:
            return data;
            break;
    }

}

// Takes the supplied date and calculates the time passed returning it as a string
// ie '1 hour ago'
function timeSince(date) {

    var seconds = Math.floor((new Date() - date) / 1000);

    var interval = Math.floor(seconds / 31536000);

    if (interval >= 1) {
        return interval + " years ago";
    }
    interval = Math.floor(seconds / 2592000);
    if (interval >= 1) {
        return interval + " months ago";
    }
    interval = Math.floor(seconds / 86400);
    if (interval >= 1) {
        return interval + " days ago";
    }
    interval = Math.floor(seconds / 3600);
    if (interval >= 1) {
        return interval + " hours ago";
    }
    interval = Math.floor(seconds / 60);
    if (interval >= 1) {
        return interval + " minutes ago";
    }
    return "Less than a minute ago";
}

// Parses text into rich text
// adds links, twitter handles
function ParseRichText(textContent) {

    var outputString = textContent;
    // replace links with a tags
    var linkExp = new RegExp("\\bhttps?://[\\S]+", "gi");
    outputString = outputString.replace(linkExp, function (match) { return '<a href="' + match + '" target="_blank" >' + match + '</a>'; });
    // replace twitter handles with a tags
    var handleExp = new RegExp("([^|\\S])(@)([a-zA-Z0-9_]{1,15})(?!\\w)", "gi");
    outputString = outputString.replace(handleExp, function (match, p1, p2, p3) { return '<a href="http://www.twitter.com/' + p3 + '" target="_blank" >' + match + '</a>'; });
    // replace twitter hash tags with links
    var hashExpr = new RegExp("(#)([a-zA-Z]{1}\\w+)", "gi");
    outputString = outputString.replace(hashExpr, function (match, p1, p2) { return '<a href="http://www.twitter.com/search?q=%23' + p2 + '&src=hash" target="_blank" >' + match + '</a>'; });

    return outputString;
}