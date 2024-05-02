
$(document).ready(function () {
    
});

function sortColumn(event) {
    var index = $(".sort-action").index($(event.currentTarget));

//    sortTable(index);
}

function sortTable(n) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;

    table = document.querySelector(".data-grid");
    switching = true;

    dir = "asc";

    while (switching) {
        switching = false;
        rows = table.getElementsByTagName("tr");

        for (i = 1; i < (rows.length - 1) ; i++) {
            shouldSwitch = false;

            x = rows[i].getElementsByTagName("TD")[n];
            y = rows[i + 1].getElementsByTagName("TD")[n];

            var xVal = x.innerText.toLowerCase();
            var yVal = y.innerText.toLowerCase();

            var xCheckBox = x.querySelector("input[type='checkbox']");
            var yCheckBox = y.querySelector("input[type='checkbox']");

            if (xCheckBox != null && yCheckBox != null) {
                xVal = $(xCheckBox).is(":checked") ? 1 : 0;
                yVal = $(yCheckBox).is(":checked") ? 1 : 0;
            }
            else if (isInt(xVal) && isInt(yVal)) {
                xVal = parseInt(xVal);
                yVal = parseInt(yVal);
            }

            if (dir === "asc") {

                if (xVal > yVal) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir === "desc") {
                if (xVal < yVal) {
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;

            switchcount++;
        } else {

            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}