const _el = el => document.querySelector(el)

$(() => {
    $("#loaderbody").addClass('d-none');

    // Mostrar css spinning durante processamento do ajax
    $(document)
        .on('ajaxStart', () => { $("#loaderbody").removeClass('d-none') })
        .on('ajaxStop', () => { $("#loaderbody").addClass('d-none') });

    const userlastnameEl = _el("#spanUserlastname");
    if (userlastnameEl) {
        var userlastname = userlastnameEl.getAttribute("data-userlastname");

        _el("#UserLink").addEventListener("mouseenter", () => TypewriterEffect("#spanUserlastname", userlastname, 15))

        _el("nav").addEventListener("mouseleave", () => BackSpaceEffect("#spanUserlastname", userlastname, 15))
    }

    let telefoneinputs = document.querySelectorAll("input.telefone");
    telefoneinputs.forEach(telefone => {
        telefone.addEventListener("input", (e) => {
            formatapplier(e.target, telformatter);
        })
    });


    let cpfinputs = document.querySelectorAll("input.cpf");
    cpfinputs.forEach((el) => {
        el.addEventListener("input", (e) => {
            formatapplier(e.target, cpfformatter);
        });
    });

    let meucepinput = document.querySelectorAll("input.cep");
    meucepinput.forEach((el) => {
        el.addEventListener("input", (e) => {
            formatapplier(e.target, cepformatter);
        });
    });
});

/* Máscaras */
function formatapplier(element, formatter) {
    let posicaoCursor = element.selectionStart;
    let beforelen = element.value.length;
    element.value = formatter(element.value);
    afterlen = element.value.length;
    posicaoCursor += afterlen - beforelen;
    element.setSelectionRange(posicaoCursor, posicaoCursor);
}

telformatter = (v) => {
    v = v.replace(/\D/g, "").substring(0, 11); //Remove tudo o que não é dígito
    v = v.replace(/^(\d{2})(\d)/g, "($1) $2"); //Coloca parênteses em volta dos dois primeiros dígitos
    v = v.replace(/(\d)(\d{4})$/, "$1-$2"); //Coloca hífen entre o quarto e o quinto dígitos
    return v;
};

cpfformatter = (v) => {
    v = v.replace(/\D/g, "").substring(0, 11); //Remove tudo o que não é dígito
    v = v.replace(/^(\d{3})(\d)/g, "$1.$2"); //Coloca ponto após os 3 primeiros dígitos
    v = v.replace(/^(\d{3}.\d{3})(\d)/g, "$1.$2");
    v = v.replace(/^(\d{3}.\d{3}.\d{3})(\d)/g, "$1-$2");
    return v;
};

cepformatter = (v) => {
    v = v.replace(/\D/g, "").substring(0, 8); //Remove tudo o que não é dígito
    v = v.replace(/^(\d{2})(\d)/g, "$1.$2"); //Coloca ponto após os 3 primeiros dígitos
    v = v.replace(/^(\d{2}.\d{3})(\d)/g, "$1-$2");
    return v;
};

TypewriterEffect = (el, text, speed, i = 0) => {
    if (_el(el).innerText.length < text.length) {
        _el(el).innerText = text.substr(0, i++);
        setTimeout(
            () => TypewriterEffect(el, text, speed, i),
            Math.floor(Math.random() * speed)
        );
    }
};

BackSpaceEffect = (el, text, speed, i = 1) => {
    if (text.length && _el(el).innerText.length > 0) {
        _el(el).innerText = text.substr(0, text.length - i++);
        setTimeout(
            () => BackSpaceEffect(el, text, speed, i),
            Math.floor(Math.random() * speed)
        );
    }
};

showInPopup = (url, title) => {
    try {
        $.ajax({
            type: 'GET',
            url: url,
            success: function (res) {
                $('#form-modal .modal-title').html(title);
                $('#form-modal .modal-body').html(res);
                $('#form-modal').modal('show');

                // to make popup draggable
                //$('.modal-dialog').draggable({
                //    handle: ".modal-header"
                //});
            },
            error: function (err) {
                if (err.status === 401) {
                    AjaxLogin();
                } else {
                    bootbox.alert("Ops, não consegui abrir o formulário. ")
                }
                console.error(err)
            }
        })
    } catch (err) {
        bootbox.alert("Ops, não consegui abrir o formulário. ")
        console.error(err)
    }
}

jQueryAjaxPost = form => {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#view-all').html(res.html)
                        $('#form-modal .modal-body').empty();
                        $('#form-modal .modal-title').empty();
                        $('#form-modal').modal('hide');
                    }
                    else
                        $('#form-modal .modal-body').html(res.html);
                },
                error: function (err) {
                    if (err.status === 401) {
                        AjaxLogin();
                    } else {
                        bootbox.alert("Ops, não consegui enviar o formulário.")
                    }
                    console.error(err)
                }
            })
        } catch (e) {
            bootbox.alert("Ops, não consegui enviar o formulário.")
            console.error(e)
        }
    }
    //to prevent default form submit event
    return false;
}

jQueryAjaxDelete = form => {
    var targetName = form.TargetName.value || "o registro";

    bootbox.confirm({
        title: `Exclusão de ${form.action.split("/")[3]}`,
        message: `Deseja realmente excluir ${targetName} ?`,
        buttons: {
            confirm: {
                label: '<i class="bi bi-trash"></i>&nbsp;&nbsp;Excluir',
                className: 'btn-danger'
            },
            cancel: {
                label: '<i class="bi bi-x-circle"></i>&nbsp;&nbsp;Cancelar',
                className: 'btn-secondary'
            }
        },
        callback: (response) => {
            if (response) {
                try {
                    $.ajax({
                        type: 'POST',
                        url: form.action,
                        data: new FormData(form),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $('#view-all').html(res.html);
                        },
                        error: function (err) {
                            if (err.status === 401)
                                AjaxLogin();
                            else
                                bootbox.alert({
                                    title: `Exclusão de ${form.action.split("/")[3]}`,
                                    message: `Ops, não consegui excluir ${targetName}`
                                });
                            console.error(err)
                        }
                    })
                } catch (err) {
                    bootbox.alert({
                        title: `Exclusão de ${form.action.split("/")[3]}`,
                        message: `Ops, não consegui excluir ${targetName}`
                    });
                    console.error(err)
                }
            }
        }
    });
    //prevent default form submit event
    return false;
}

AjaxLogin = (title = 'Acesso Negado', msg = 'Sua conexão expirou.', btn = 'Reconectar') => {
    bootbox.confirm({
        title: `<i class="bi bi-dash-circle"></i>&nbsp;&nbsp;${title}`,
        message: msg,
        buttons: {
            confirm: {
                label: `<i class="bi bi-box-arrow-in-right"></i>&nbsp;&nbsp;${btn}`,
                className: 'btn-primary'
            },
            cancel: {
                label: '<i class="bi bi-x-circle"></i>&nbsp;&nbsp;Cancelar',
                className: 'btn-secondary'
            }
        },
        callback: (response) => {
            if (response) {
                AjaxLoginFormGetter('/Usuario/AjaxLogin', 'Login de Usuário')
            }
        }
    });
}

// Get the form and show in popup
AjaxLoginFormGetter = (url, title) => {
    try {
        $.ajax({
            type: 'GET',
            url: url,
            success: function (res) {
                $('#ajaxloginform-modal .modal-title').html(title);
                $('#ajaxloginform-modal .modal-body').html(res);
                $('#ajaxloginform-modal').modal('show');
            },
            error: function (err) {
                bootbox.alert("Ocorreu um erro ao obter o formulário de Login: ")
                console.error(err)
            }
        })
    } catch (e) {
        bootbox.alert("Ocorreu um erro ao abrir o formulário de Login: ")
        console.error(e)
    }
}

jQueryAjaxLoginPost = form => {
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#ajaxloginform-modal .modal-body').empty();
                        $('#ajaxloginform-modal .modal-title').empty();
                        $('#ajaxloginform-modal').modal('hide');
                    } else
                        $('#ajaxloginform-modal .modal-body').html(res.html);
                },
                error: function (err) {
                    bootbox.alert("Ocorreu um erro ao postar o formulário de Login: ")
                    console.error(err)
                }
            })
        } catch (e) {
            bootbox.alert("Ocorreu um erro ao postar o formulário de Login: ")
            console.error(e)
        }
    }
    //to prevent default form submit event
    return false;
}

const Language = {
    "emptyTable": "Nenhum registro encontrado",
    "info": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
    "infoFiltered": "(Filtrados de _MAX_ registros)",
    "infoThousands": ".",
    "loadingRecords": "Carregando...",
    "zeroRecords": "Nenhum registro encontrado",
    "search": "Pesquisar",
    "paginate": {
        "next": "Próximo",
        "previous": "Anterior",
        "first": "Primeiro",
        "last": "Último"
    },
    "aria": {
        "sortAscending": ": Ordenar colunas de forma ascendente",
        "sortDescending": ": Ordenar colunas de forma descendente"
    },
    "select": {
        "rows": {
            "_": "Selecionado %d linhas",
            "1": "Selecionado 1 linha"
        },
        "cells": {
            "1": "1 célula selecionada",
            "_": "%d células selecionadas"
        },
        "columns": {
            "1": "1 coluna selecionada",
            "_": "%d colunas selecionadas"
        }
    },
    "buttons": {
        "copySuccess": {
            "1": "Uma linha copiada com sucesso",
            "_": "%d linhas copiadas com sucesso"
        },
        "collection": "Coleção  <span class=\"ui-button-icon-primary ui-icon ui-icon-triangle-1-s\"><\/span>",
        "colvis": "Visibilidade da Coluna",
        "colvisRestore": "Restaurar Visibilidade",
        "copy": "Copiar",
        "copyKeys": "Pressione ctrl ou u2318 + C para copiar os dados da tabela para a área de transferência do sistema. Para cancelar, clique nesta mensagem ou pressione Esc..",
        "copyTitle": "Copiar para a Área de Transferência",
        "csv": "CSV",
        "excel": "Excel",
        "pageLength": {
            "-1": "Mostrar todos os registros",
            "_": "Mostrar %d registros"
        },
        "pdf": "PDF",
        "print": "Imprimir",
        "createState": "Criar estado",
        "removeAllStates": "Remover todos os estados",
        "removeState": "Remover",
        "renameState": "Renomear",
        "savedStates": "Estados salvos",
        "stateRestore": "Estado %d",
        "updateState": "Atualizar"
    },
    "autoFill": {
        "cancel": "Cancelar",
        "fill": "Preencher todas as células com",
        "fillHorizontal": "Preencher células horizontalmente",
        "fillVertical": "Preencher células verticalmente"
    },
    "lengthMenu": "Exibir_MENU_ resultados por página",
    "searchBuilder": {
        "add": "Adicionar Condição",
        "button": {
            "0": "Construtor de Pesquisa",
            "_": "Construtor de Pesquisa (%d)"
        },
        "clearAll": "Limpar Tudo",
        "condition": "Condição",
        "conditions": {
            "date": {
                "after": "Depois",
                "before": "Antes",
                "between": "Entre",
                "empty": "Vazio",
                "equals": "Igual",
                "not": "Não",
                "notBetween": "Não Entre",
                "notEmpty": "Não Vazio"
            },
            "number": {
                "between": "Entre",
                "empty": "Vazio",
                "equals": "Igual",
                "gt": "Maior Que",
                "gte": "Maior ou Igual a",
                "lt": "Menor Que",
                "lte": "Menor ou Igual a",
                "not": "Não",
                "notBetween": "Não Entre",
                "notEmpty": "Não Vazio"
            },
            "string": {
                "contains": "Contém",
                "empty": "Vazio",
                "endsWith": "Termina Com",
                "equals": "Igual",
                "not": "Não",
                "notEmpty": "Não Vazio",
                "startsWith": "Começa Com",
                "notContains": "Não contém",
                "notStartsWith": "Não começa com",
                "notEndsWith": "Não termina com"
            },
            "array": {
                "contains": "Contém",
                "empty": "Vazio",
                "equals": "Igual à",
                "not": "Não",
                "notEmpty": "Não vazio",
                "without": "Não possui"
            }
        },
        "data": "Data",
        "deleteTitle": "Excluir regra de filtragem",
        "logicAnd": "E",
        "logicOr": "Ou",
        "title": {
            "0": "Construtor de Pesquisa",
            "_": "Construtor de Pesquisa (%d)"
        },
        "value": "Valor",
        "leftTitle": "Critérios Externos",
        "rightTitle": "Critérios Internos"
    },
    "searchPanes": {
        "clearMessage": "Limpar Tudo",
        "collapse": {
            "0": "Painéis de Pesquisa",
            "_": "Painéis de Pesquisa (%d)"
        },
        "count": "{total}",
        "countFiltered": "{shown} ({total})",
        "emptyPanes": "Nenhum Painel de Pesquisa",
        "loadMessage": "Carregando Painéis de Pesquisa...",
        "title": "Filtros Ativos",
        "showMessage": "Mostrar todos",
        "collapseMessage": "Fechar todos"
    },
    "thousands": ".",
    "datetime": {
        "previous": "Anterior",
        "next": "Próximo",
        "hours": "Hora",
        "minutes": "Minuto",
        "seconds": "Segundo",
        "amPm": [
            "am",
            "pm"
        ],
        "unknown": "-",
        "months": {
            "0": "Janeiro",
            "1": "Fevereiro",
            "10": "Novembro",
            "11": "Dezembro",
            "2": "Março",
            "3": "Abril",
            "4": "Maio",
            "5": "Junho",
            "6": "Julho",
            "7": "Agosto",
            "8": "Setembro",
            "9": "Outubro"
        },
        "weekdays": [
            "Domingo",
            "Segunda-feira",
            "Terça-feira",
            "Quarta-feira",
            "Quinte-feira",
            "Sexta-feira",
            "Sábado"
        ]
    },
    "editor": {
        "close": "Fechar",
        "create": {
            "button": "Novo",
            "submit": "Criar",
            "title": "Criar novo registro"
        },
        "edit": {
            "button": "Editar",
            "submit": "Atualizar",
            "title": "Editar registro"
        },
        "error": {
            "system": "Ocorreu um erro no sistema (<a target=\"\\\" rel=\"nofollow\" href=\"\\\">Mais informações<\/a>)."
        },
        "multi": {
            "noMulti": "Essa entrada pode ser editada individualmente, mas não como parte do grupo",
            "restore": "Desfazer alterações",
            "title": "Multiplos valores",
            "info": "Os itens selecionados contêm valores diferentes para esta entrada. Para editar e definir todos os itens para esta entrada com o mesmo valor, clique ou toque aqui, caso contrário, eles manterão seus valores individuais."
        },
        "remove": {
            "button": "Remover",
            "confirm": {
                "_": "Tem certeza que quer deletar %d linhas?",
                "1": "Tem certeza que quer deletar 1 linha?"
            },
            "submit": "Remover",
            "title": "Remover registro"
        }
    },
    "decimal": ",",
    "stateRestore": {
        "creationModal": {
            "button": "Criar",
            "columns": {
                "search": "Busca de colunas",
                "visible": "Visibilidade da coluna"
            },
            "name": "Nome:",
            "order": "Ordernar",
            "paging": "Paginação",
            "scroller": "Posição da barra de rolagem",
            "search": "Busca",
            "searchBuilder": "Mecanismo de busca",
            "select": "Selecionar",
            "title": "Criar novo estado",
            "toggleLabel": "Inclui:"
        },
        "emptyStates": "Nenhum estado salvo",
        "removeConfirm": "Confirma remover %s?",
        "removeJoiner": "e",
        "removeSubmit": "Remover",
        "removeTitle": "Remover estado",
        "renameButton": "Renomear",
        "renameLabel": "Novo nome para %s:",
        "renameTitle": "Renomear estado",
        "duplicateError": "Já existe um estado com esse nome!",
        "emptyError": "Não pode ser vazio!",
        "removeError": "Falha ao remover estado!"
    },
    "infoEmpty": "Mostrando 0 até 0 de 0 registro(s)",
    "processing": "Carregando...",
    "searchPlaceholder": "Buscar registros"
}