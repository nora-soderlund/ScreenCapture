const canvas = document.getElementById("canvas");
const context = canvas.getContext("2d");

let image = null;

function set(base64) {
    image = new Image();

    image.onload = () => {
        canvas.width = image.width;
        canvas.height = image.height;

        render(null, null);

        get();
    };

    image.src = "data:image/png;base64," + base64;
};

function get() {
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


        const result = document.createElement("canvas");
        result.width = rectangle[0];
        result.height = rectangle[1];

        const context = result.getContext("2d");
        context.drawImage(image, mousestart[0], mousestart[1], result.width, result.height, 0, 0, result.width, result.height);

        window.location = "result://" + result.toDataURL().split(';base64,')[1];
    });
};

function render(start, rectangle) {
    if(start == null || rectangle == null) {
        context.fillStyle = "rgba(0, 0, 0, .5)";
        context.fillRect(0, 0, canvas.width, canvas.height);

        context.globalCompositeOperation = "destination-over";
        context.drawImage(image, 0, 0, image.width, image.height);

        return;
    }

    context.save();
    
    context.clearRect(0, 0, canvas.width, canvas.height);

    context.fillStyle = "rgba(0, 0, 0, .5)";
    context.fillRect(0, 0, canvas.width, canvas.height);
    
    context.globalCompositeOperation = "destination-out";
    context.fillStyle = "black";
    context.fillRect(start[0], start[1], rectangle[0], rectangle[1]);
    
    context.globalCompositeOperation = "destination-over";
    context.drawImage(image, 0, 0, image.width, image.height);

    context.restore();
};
