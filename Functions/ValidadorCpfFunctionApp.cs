using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ValidadorCpfFunction
{
    private readonly ILogger _logger;

    public ValidadorCpfFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ValidadorCpfFunction>();
    }

    [Function("ValidaCpf")]
    public async Task<string> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] string cpf,
        FunctionContext executionContext)
    {
        _logger.LogInformation("Validando CPF: {cpf}", cpf);

        // Validação básica do CPF
        var isValid = ValidarCpf(cpf);
        return isValid ? "CPF Válido" : "CPF Inválido";
    }

    private bool ValidarCpf(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = cpf.Replace(".", "").Replace("-", "");

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        // Cálculo dos dígitos verificadores
        var digitos = cpf.Substring(0, 9);
        var digito1 = CalcularDigito(digitos, 10);
        var digito2 = CalcularDigito(digitos + digito1, 11);

        return cpf.EndsWith(digito1.ToString() + digito2.ToString());
    }

    private int CalcularDigito(string baseCpf, int peso)
    {
        int soma = 0;
        for (int i = 0; i < baseCpf.Length; i++)
        {
            soma += int.Parse(baseCpf[i].ToString()) * (peso - i);
        }

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}