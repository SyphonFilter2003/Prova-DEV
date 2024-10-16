using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using Joao2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();

app.MapGet("/Joao2", () => "Prova-DEV");

app.MapPost("/Joao2/funcionario/cadastrar", async ([FromBody] Funcionario funcionario, AppDataContext ctx) => {

    ctx.Add(funcionario);
    await ctx.SaveChangesAsync();

    return Results.Created("", funcionario);
});

app.MapGet("/Joao2/funcionario/listar", (AppDataContext ctx) => {
    if (ctx.Funcionarios.Any())
        return Results.Ok(ctx.Funcionarios.ToList());

    return Results.NotFound("Nenhum funcionário encontrado.");
});

app.MapPost("/Joao2/folha/cadastrar", async ([FromBody] Folha folha, AppDataContext ctx) => {

    Funcionario? funcionario = ctx.Funcionarios.Find(folha.FuncionarioId);

    if (funcionario == null)
        return Results.NotFound("Funcionário não encontrado no banco de dados.");
    
    folha.Funcionario = funcionario;

    float salario_bruto = folha.Quantidade * folha.Valor / folha.Mes;
    float salario_liquido = salario_bruto;
    float imposto_renda = 0.0f;
    float inss = 0.0f;

    if (salario_bruto <= 1_903.98 && salario_bruto >= 2_826.65)
        imposto_renda = (float) (salario_bruto * 0.075);
    
    if (salario_bruto >= 2_826.66 && salario_bruto <= 3_751.05)
        imposto_renda = (float) (salario_bruto * 0.15);

    if (salario_bruto >= 3_751.06 && salario_bruto <= 4_664.68)
        imposto_renda = (float) (salario_bruto * 0.225);
    
    if (salario_bruto > 4_664.68)
        imposto_renda = (float) (salario_bruto * 0.275);

    if (salario_bruto <= 1_693.72)
        inss = (float) (salario_bruto * 0.08);

    if (salario_bruto >= 1_693.73 && salario_bruto <= 2_822.90)
        inss = (float) (salario_bruto * 0.09);

    if (salario_bruto >= 2_822.91 && salario_bruto <= 5_645.80)
        inss = (float) (salario_bruto * 0.11);

    if (salario_bruto > 5_645.81)
        inss = 621.03f;

    float fgts = (float) (salario_bruto * 0.08);


    folha.SalarioBruto = salario_bruto;
    salario_liquido = salario_bruto - imposto_renda - inss;

    folha.ImpostoIrrf = imposto_renda;
    folha.ImpostoInss = inss;
    folha.ImpostoFgts = fgts;
    folha.SalarioLiquido = salario_liquido;

    ctx.Add(folha);
    await ctx.SaveChangesAsync();

    return Results.Created("", folha);
});

app.MapGet("/Joao2/folha/listar", (AppDataContext ctx) => {
    if (ctx.Funcionarios.Any())
        return Results.Ok(ctx.Folhas.ToList());

    return Results.NotFound("Nenhuma folha cadastrada!");
});

app.MapGet("/Joao2/folha/buscar/{cpf}/{mes}/{ano}", ([FromRoute] string cpf, [FromRoute] int mes, [FromRoute] int ano,
AppDataContext ctx) => {
    Funcionario? funcionario = ctx.Funcionarios.FirstOrDefault(f => f.Cpf == cpf); 

    Folha? folha = ctx.Folhas.Include(f => f.Funcionario).FirstOrDefault(f => f.Funcionario != null && f.Funcionario.Cpf == cpf && f.Mes == mes && f.Ano == ano);

    if (folha != null)
        return Results.Ok(folha);

    return Results.NotFound("Nenhuma folha cadastrada!");
});

app.Run();