$(function () {

    // Declare a proxy to reference the hub.
    var blas = $.connection.bLAS;
    var i = 0;
    var data = [];
    var displayOutput = "[";
    // Start the connection.
    $.connection.hub.start().done(function () {
        alert('Now connected, connection ID=' + $.connection.hub.id)
        $('#send').click(function () {
            var mat1JSON = $('#mat1').val();
            var mat2JSON = $('#mat2').val();
            // Call the QR decomposition method on the hub.       
            blas.server.blas1(mat1JSON, mat2JSON, $.connection.hub.id);
        });
    });
 
    // Create a function that the hub can call to store the solution.
    blas.client.store = function (product) {
        var productObj = JSON.parse(product);
        data[i] = productObj;
        i++;
    };
    //Create a function that hub can call to display the final output
    blas.client.displayOutput = function () {
        for (i = 0; i < data.length; i++) {
            displayOutput += '[' + data[i] + ']';
            if (i != data.length - 1) {
                displayOutput += ',';
            }
            else
            {
                displayOutput += ']';
            }
               
        }
        document.getElementById("Product").innerHTML = 'The output is ' + displayOutput;
        displayOutput = "[";
        i = 0;
        data = [];
    };

});
