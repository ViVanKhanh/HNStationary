$(document).ready(function () {
    ShowCount(); // Gọi khi trang vừa load

    $('body').on('click', '.btnAddToCard', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;
        var tQuantity = $('#quantity_value').text();

        if (tQuantity !== '') {
            quantity = parseInt(tQuantity);
        }

        $.ajax({
            url: '/shoppingcard/addtocard',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                if (rs.success) {
                    $('#checkout_items').html(rs.Count);
                    alert(rs.msg);
                } else {
                    alert(rs.msg);
                }
            },
            error: function () {
                alert("Có lỗi xảy ra khi thêm vào giỏ hàng");
            }
        });
    });
    $('body').off('click', '.btnDeleteAll').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm('Bạn có muốn xóa tất cả sản phẩm này không?');
        if (conf) {
            DeleteAll();
            LoadCard();
        }
    });
    $('body').off('click', '.btnUpdate').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $("#Quantity_" + id).val();

        Update(id, quantity)
    });
$('body').off('click', '.btnDelete').on('click', '.btnDelete', function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    var conf = confirm('Bạn có muốn xóa sản phẩm này không?');
    if (conf) {
        $.ajax({
            url: '/shoppingcard/Delete',
            type: 'POST',
            data: { id: id },
            success: function (rs) {
                if (rs.success) {
                    $('#checkout_items').html(rs.Count);
                    LoadCard(); // KHÔNG dùng .remove()
                }
            },
            error: function () {
                alert("Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng");
            }
        });
    }
});


});

function ShowCount() {
    $.ajax({
        url: '/shoppingcard/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}

function DeleteAll() {
    $.ajax({
        url: '/shoppingcard/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                LoadCard();
            }
        }
    });
}

function Update(id, quantity) {
    $.ajax({
        url: '/shoppingcard/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            if (rs.Success) {
                LoadCard();
            }
        }
    });
}
function LoadCard() {
    $.ajax({
        url: '/shoppingcard/Partial_Item_Card',
        type: 'GET',
        success: function (rs) {
            $('#Load_data').html(rs);
        }
    });
}
