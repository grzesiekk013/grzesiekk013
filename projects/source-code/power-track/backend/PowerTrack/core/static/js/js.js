// -- -- -- Mechanizm dodawania kanałów -- -- -- //
document.addEventListener("DOMContentLoaded",function() {

    // -- -- -- Globalny słuchacz dla klinięć add-port-row-btn -- -- --//
    document.addEventListener('click', function(event) {
        const addRow = event.target.closest('.add-port-row-btn');
        if (addRow) {
            const modal = addRow.closest('.modal');
            const body = modal.querySelector('.ports-table-body');
            const template = document.getElementById('port-row-template');

            if (body && template) {
                const newRow = template.content.cloneNode(true);
                body.appendChild(newRow);
            }
        }

    });

    // -- -- -- Globalny mechanizm usuwania wiersza -- -- -- //
    document.addEventListener('click', function(event) {
        const deleteBtn = event.target.closest('.del-port-row-btn');
        if (deleteBtn) {
            const rowToDelete = deleteBtn.closest('tr');
            if (rowToDelete) {
                rowToDelete.remove();
            }
        }
    });


});

// -- -- -- Wyskakiwanie Powiadomień -- -- -- //

const option = {
    autohide: true,
    delay: 10000
}
document.addEventListener('DOMContentLoaded', function () {
    // Źródło: https://getbootstrap.com/docs/5.3/components/toasts/
    const toastElList = document.querySelectorAll('.toast')
    const toastList = [...toastElList].map(toastEl => {
        let t = new bootstrap.Toast(toastEl, option);
        t.show();
        return t;
    });
});

// -- -- -- Mechanizm wyświetlania wykresu na dużym oknie -- -- //

let fullscreenChart = null;

function openFullscreen(chart_id, title) {
    const org_canvas = document.getElementById(chart_id);
    const org_chart = Chart.getChart(org_canvas);

    if (!org_chart) {
        console.log("Nie znaleziono wykresu o #id: "+chart_id);
        return;
    }

    document.getElementById('chart_modal_title').innerText = title;

    const modal_element = document.getElementById('chart_modal');
    const modal = new bootstrap.Modal(modal_element);
    modal.show();

    modal_element.addEventListener('shown.bs.modal', function() {
        const canvas = document.getElementById('chart_full_screen_canvas').getContext('2d');

       if (fullscreenChart) {
        fullscreenChart.destroy();
       }

       fullscreenChart = new Chart(canvas, {
            type: org_chart.config.type,
            data: JSON.parse(JSON.stringify(org_chart.data)),
            options: {
                ...org_chart.options,
                maintainAspectRatio: false,
                plugins: {
                    ...org_chart.options.plugins,
                    legend: {
                        display: true,
                        position: 'bottom'
                    }
                }
            }
       });
    }, {once: true});

}


