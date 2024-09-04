$(() => {
    var valorselecionado = $("#CidadeId").data("id")
    var cidadeSel = document.getElementById("CidadeId")

    var carregarCidades = (url) => {
        cidadeSel.options.length = 1;

        $.ajax({
            url: url,
            type: "GET",
            success: function (data, status) {
                for (var i = 0; i < data.length; i++) {
                    cidadeSel.options[cidadeSel.options.length] = new Option(data[i]["text"], data[i]["value"], false, data[i]["value"] == valorselecionado);
                }
                valorselecionado = "";
            },
            error: function (err) {
                alert('Ops! Ocorreu um erro ao carregar as cidades:' + err);
            }
        })

    };

    $("#EstadoId").on("change", (e) => {
        carregarCidades('/Cidade/GetOptions/' + e.target.value);
    })

    $("#EstadoId").trigger("change");
});
