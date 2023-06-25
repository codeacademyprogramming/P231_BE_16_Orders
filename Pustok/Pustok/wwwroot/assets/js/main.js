$(document).on("click", ".modal-btn", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url).then(response => {
        if (response.ok) {
            return response.text()
        }
        else {
            alert("Xeta bas verdi")
            return
        }
    })
        .then(data => {
            $("#quickModal .modal-dialog").html(data)
        })
    $("#quickModal").modal("show")
})

$(document).on("click", ".basket-add-btn", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url).then(response => {
        if (!response.ok) {
            alert("Xeta bas verdi")
        }
        else return response.text()
    }).then(data => {
        $("#basketCart").html(data)
    })
})


//let btns = document.querySelectorAll(".modal-btn");

//btns.forEach(x => {
//    //x.addEventListener("click", function (e) {
//    //    alert("salam")
//    //})
//    //x.onclick = function (e) {
//    //    alert("salfdfam")
//    //}
//})

