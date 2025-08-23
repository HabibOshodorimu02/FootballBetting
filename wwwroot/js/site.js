// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Auto-dismiss alerts after 5 seconds
$(document).ready(function () {
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Add confirmation for admin actions
    $('.btn-success[type="submit"]').click(function (e) {
        if ($(this).text().includes('Update Result')) {
            if (!confirm('Are you sure you want to update this match result? This action will process all predictions and cannot be undone.')) {
                e.preventDefault();
            }
        }
    });

    // Real-time score validation
    $('select[name="homeScore"], select[name="awayScore"]').change(function () {
        var homeScore = $('select[name="homeScore"]').val();
        var awayScore = $('select[name="awayScore"]').val();

        if (homeScore && awayScore) {
            var result = '';
            if (parseInt(homeScore) > parseInt(awayScore)) {
                result = 'Home Win';
            } else if (parseInt(awayScore) > parseInt(homeScore)) {
                result = 'Away Win';
            } else {
                result = 'Draw';
            }

            // You can add visual feedback here
            console.log('Predicted result: ' + result);
        }
    });
});
