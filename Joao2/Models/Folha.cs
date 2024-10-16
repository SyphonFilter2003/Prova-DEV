using System;

namespace Joao2.Models;

public class Folha {
	public int Id { get; set; }
	public float Valor { get; set; }
    public int Quantidade { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public float SalarioBruto { get; set; }
    public float ImpostoIrrf { get; set; }
    public float ImpostoInss { get; set; }
    public float ImpostoFgts { get; set; }
    public float SalarioLiquido { get; set; }
    public Funcionario? Funcionario { get; set; }
    public int FuncionarioId { get; set; }
}