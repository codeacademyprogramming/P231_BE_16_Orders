$(document).on("click", ".delete-btn", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(url).then(response => {
                console.log(response)
                if (response.ok) {
                    Swal.fire(
                        'Deleted!',
                        'Your file has been deleted.',
                        'success'
                    ).then(() => {
                        window.location.reload();
                    })
                }
                else if (response.status == 400) {
                    alert("Elaqeli datalari var, sile bilmezsiniz!")
                }
                else {
                    alert("xeta bas verdi")
                }
            })
           
        }
    })
})