const canvas = document.getElementById("canvas");
let context = canvas.getContext("2d");

const bounds = canvas.parentElement.getBoundingClientRect();

canvas.width = bounds.width;
canvas.height = bounds.height;

let mousestart = null, mousedown = false;

canvas.addEventListener("mousedown", (event) => {
    mousestart = [ event.pageX, event.pageY ];

    mousedown = true;
});

canvas.addEventListener("mousemove", (event) => {
    if(mousedown == false)
        return;

    let mouseend = [ event.pageX, event.pageY ];
    let rectangle = [ mouseend[0] - mousestart[0], mouseend[1] - mousestart[1] ];

    render(mousestart, rectangle);
});

canvas.addEventListener("mouseup", (event) => {
    if(mousedown == false)
        return;

    mousedown = false;

    let mouseend = [ event.pageX, event.pageY ];
    let rectangle = [ mouseend[0] - mousestart[0], mouseend[1] - mousestart[1] ];

    render(mousestart, rectangle);

    window.location = "result://" + mousestart[0] + "," + mousestart[1] + "," + rectangle[0] + "," + rectangle[1];
});

function render(start, rectangle) {
    if(start == null || rectangle == null) {
        context.fillStyle = "rgba(0, 0, 0, .5)";
        context.fillRect(0, 0, canvas.width, canvas.height);
        return;
    }

    context.save();
    
    context.clearRect(0, 0, canvas.width, canvas.height);

    context.fillStyle = "rgba(0, 0, 0, .5)";
    context.fillRect(0, 0, canvas.width, canvas.height);
    
    context.globalCompositeOperation = "destination-out";
    context.fillStyle = "black";
    context.fillRect(start[0], start[1], rectangle[0], rectangle[1]);

    context.restore();
};

render();
