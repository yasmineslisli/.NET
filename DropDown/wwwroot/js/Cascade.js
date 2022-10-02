    $(document).ready(function () {
    GetProgramme();
    $('#Programme').change(function () {
        var id = $(this).val();
        $('#Projet').empty();
        $('#Projet').append('<Option>--Selectionner un projet--</Option>');
        $.ajax({

            url: '/Cascade/Projet?id=' + id,
            success: function (result) {
                $.each(result, function (i, data) {
                    $('#Projet').append('<Option value=' + data.id + '>' + data.name + '</Option>');
                });
            }

        });
    });
        $('#Projet').change(function () {
            var id = $(this).val();
            $('#ActionProj').empty();
            $('#ActionProj').append('<Option>--Selectionner une action--</Option>');
            $.ajax({

                url: '/Cascade/ActionProj?id=' + id,
                success: function (result) {
                    $.each(result, function (i, data) {
                        $('#ActionProj').append('<Option value=' + data.id + '>' + data.name + '</Option>');
                    });
                }

            });
        });
});
        function GetProgramme() {
            $.ajax({
                url: '/Cascade/Programme',
                success: function (result) {
                    $('#Programme').append('<Option>--Selectionner un programme--</Option>');
                    $.each(result, function (i, data) {
                        $('#Programme').append('<Option value=' + data.id + '>' + data.name + '</Option>');
                    });
                }
            });
        }



