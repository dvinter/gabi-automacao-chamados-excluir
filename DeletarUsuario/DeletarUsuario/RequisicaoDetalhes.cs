using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletarUsuario
{
    public partial class RequisicaoDetalhes
    {
        [JsonProperty("Conjuntos")]
        public Conjuntos Conjuntos { get; set; }

        [JsonProperty("UrlDadosAdicionais")]
        public Uri UrlDadosAdicionais { get; set; }

        [JsonProperty("UrlAcoes")]
        public Uri UrlAcoes { get; set; }

        [JsonProperty("UrlHistorico")]
        public Uri UrlHistorico { get; set; }

        [JsonProperty("UrlRequisicoesVinculadas")]
        public Uri UrlRequisicoesVinculadas { get; set; }

        [JsonProperty("MensagensErro")]
        public object[] MensagensErro { get; set; }
    }

    public partial class Conjuntos
    {
        [JsonProperty("Detalhes Do Pedido")]
        public DetalhesDoPedido DetalhesDoPedido { get; set; }

        [JsonProperty("Agendamento do Pedido")]
        public AgendamentoDoPedido AgendamentoDoPedido { get; set; }

        [JsonProperty("Participantes")]
        public Participantes Participantes { get; set; }

        [JsonProperty("IdsParticipantes")]
        public IdsParticipantes IdsParticipantes { get; set; }

        [JsonProperty("Causa do Problema")]
        public CausaDoProblema CausaDoProblema { get; set; }

        [JsonProperty("Pendente Fornecedor?")]
        public PendenteFornecedor PendenteFornecedor { get; set; }

        [JsonProperty("Avaliação do Fornecedor.")]
        public AvaliaçãoDoFornecedor AvaliaçãoDoFornecedor { get; set; }

        [JsonProperty("DADOS DO USUÁRIO A SER EXCLUÍDO")]
        public DadosDoUsuarioASerExcluido DadosUsuarioBloquear { get; set; }

        [JsonProperty("Dados do Cliente")]
        public DadosDoCliente DadosDoCliente { get; set; }

        [JsonProperty("Acesso Sistemas")]
        public AcessoSistemas AcessoSistemas { get; set; }

        [JsonProperty("Acesso a Telefonia?")]
        public AcessoATelefonia AcessoATelefonia { get; set; }

        [JsonProperty("Acessos a Rede")]
        public AcessosARede AcessosARede { get; set; }
    }

    public partial class AcessoATelefonia
    {
        [JsonProperty("3.1 - Tipo de Operação")]
        public string TipoDeOperação { get; set; }

        [JsonProperty("3.2 - Classe de Ligação (Efetuar)")]
        public string ClasseDeLigaçãoEfetuar { get; set; }

        [JsonProperty("3.3 - Senha")]
        public string Senha { get; set; }
    }

    public partial class AcessoSistemas
    {
        [JsonProperty("2.1 - SAP")]
        public bool Sap { get; set; }

        [JsonProperty("2.1.2 - Informe as Funções SAP")]
        public string InformeAsFunçõesSap { get; set; }

        [JsonProperty("2.2 - Outros")]
        public bool Outros { get; set; }

        [JsonProperty("2.3 - Informe os sistemas")]
        public string InformeOsSistemas { get; set; }
    }

    public partial class AcessosARede
    {
        [JsonProperty("4.1 - Acesso a pasta de Rede")]
        public string AcessoAPastaDeRede { get; set; }

        [JsonProperty("4.1.2 - Pastas de rede")]
        public string PastasDeRede { get; set; }

        [JsonProperty("4.2 - Acesso a internet")]
        public string AcessoAInternet { get; set; }

        [JsonProperty("4.4 - Privilégio de Internet")]
        public string PrivilegioDeInternet { get; set; }

        [JsonProperty("4.3 - Email")]
        public string Email { get; set; }

        [JsonProperty("4.5 - Grupo de Email")]
        public string GrupoDeEmail { get; set; }
    }

    public partial class AgendamentoDoPedido
    {
        [JsonProperty("DataAbertura")]
        public DateTimeOffset DataAbertura { get; set; }

        [JsonProperty("DataFimPrevisto")]
        public DateTimeOffset DataFimPrevisto { get; set; }

        [JsonProperty("DataFimReal")]
        public object DataFimReal { get; set; }

        [JsonProperty("DataInicioPrevisto")]
        public DateTimeOffset DataInicioPrevisto { get; set; }

        [JsonProperty("DataInicioReal")]
        public object DataInicioReal { get; set; }
    }

    public partial class AvaliaçãoDoFornecedor
    {
        [JsonProperty("Requer Avaliação de Fornecedor")]
        public string RequerAvaliaçãoDeFornecedor { get; set; }

        [JsonProperty("Nome do Fornecedor:")]
        public string NomeDoFornecedor { get; set; }

        [JsonProperty("Como você avalia a qualidade técnica do fornecedor")]
        public string ComoVoceAvaliaAQualidadeTecnicaDoFornecedor { get; set; }

        [JsonProperty("Como você avalia o prazo de solução do Fornecedor?")]
        public string ComoVoceAvaliaOPrazoDeSolucaoDoFornecedor { get; set; }

        [JsonProperty("Como você avalia cordialidade no atendimento?")]
        public string ComoVoceAvaliaCordialidadeNoAtendimento { get; set; }

        [JsonProperty("Comentários:")]
        public string Comentarios { get; set; }
    }

    public partial class DadosDoUsuarioASerExcluido
    {
        [JsonProperty("Nome Completo")]
        public string NomeCompleto { get; set; }

        [JsonProperty("Login do Usuário")]
        public string Login { get; set; }        
    }

    public partial class CausaDoProblema
    {
        [JsonProperty("Causa do Problema")]
        public string CausaDoProblemaCausaDoProblema { get; set; }

        [JsonProperty("Valor da Proposta")]
        public string ValorDaProposta { get; set; }

        [JsonProperty("Impactou a receita ou na operação por mais de 2h?")]
        public string ImpactouAReceitaOuNaOperaçãoPorMaisDe2H { get; set; }

        [JsonProperty("Informe o número da requisição")]
        public string InformeONumeroDaRequisição { get; set; }
    }

    public partial class DadosDoCliente
    {
        [JsonProperty("1.1 - Nome Completo")]
        public string NomeCompleto { get; set; }

        [JsonProperty("1.2 - CPF")]
        public string Cpf { get; set; }

        [JsonProperty("1.3 - Data de Nascimento")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string DataDeNascimento { get; set; }

        [JsonProperty("1.4 - Divisões")]
        public string Divisoes { get; set; }

        [JsonProperty("1.4.1 - Empresas Holding")]
        public string EmpresaHolding { get; set; }

        [JsonProperty("1.4.1 - Divisão Logística")]
        public string DivisaoLogistica { get; set; }

        [JsonProperty("1.4.1 - Divisão Comércio")]
        public string DivisaoComercio { get; set; }

        [JsonProperty("1.5 - Departamento")]
        public string Departamento { get; set; }

        [JsonProperty("1.6 - Cargo")]
        public string Cargo { get; set; }

        [JsonProperty("1.7 - Contato do cliente (telefone)")]
        public string ContatoDoClienteTelefone { get; set; }

        [JsonProperty("1.8 - Nome do Gestor do Cliente")]
        public string NomeDoGestorDoCliente { get; set; }

        [JsonProperty("1.9 - Centro de Custo")]
        public string CentroDeCusto { get; set; }

        [JsonProperty("1.10 - Cidade/Estado")]
        public string CidadeEstado { get; set; }

        [JsonProperty("1.11 Prestador de Serviço?")]
        public bool PestadorServico { get; set; }

        [JsonProperty("1.12 Data de Bloqueio da conta de rede:")]
        public string DataBloqueio { get; set; }
    }

    public partial class DetalhesDoPedido
    {
        [JsonProperty("Assunto")]
        public string Assunto { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("Atividade")]
        public string Atividade { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("UltimaAcao")]
        public string UltimaAcao { get; set; }

        [JsonProperty("UltimaAcaoQuando")]
        public string UltimaAcaoQuando { get; set; }

        [JsonProperty("UltimaAcaoQuem")]
        public string UltimaAcaoQuem { get; set; }

        [JsonProperty("FormularioId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long FormularioId { get; set; }

        [JsonProperty("FormularioStatusId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long FormularioStatusId { get; set; }

        [JsonProperty("ProcessoId")]
        public string ProcessoId { get; set; }

        [JsonProperty("Prazo")]
        public string Prazo { get; set; }

        [JsonProperty("Anexos")]
        public object[] Anexos { get; set; }

        [JsonProperty("Telefone de Contato")]
        public string TelefoneDeContato { get; set; }

        [JsonProperty("Criticidade")]
        public string Criticidade { get; set; }
    }

    public partial class Participantes
    {
        [JsonProperty("SYS")]
        public string Sys { get; set; }

        [JsonProperty("Aberto por")]
        public string AbertoPor { get; set; }

        [JsonProperty("Solicitante")]
        public string Solicitante { get; set; }

        [JsonProperty("Beneficiado")]
        public string Beneficiado { get; set; }

        [JsonProperty("Grupo Solucionador")]
        public string GrupoSolucionador { get; set; }

        [JsonProperty("Controlador")]
        public string Controlador { get; set; }

        [JsonProperty("Adm Form")]
        public string AdmForm { get; set; }

        [JsonProperty("Grupo SAN")]
        public string GrupoSan { get; set; }

        [JsonProperty("Chatbot-Watson")]
        public string ChatbotWatson { get; set; }

        [JsonProperty("Taskbot")]
        public string Taskbot { get; set; }
    }

    public class IdsParticipantes
    {
        public SYS SYS { get; set; }

        public REG REG { get; set; }

        [JsonProperty("ANTE")]
        public ANTE SolicitanteDados { get; set; }

        [JsonProperty("BEN")]
        public BEN BeneficiadoDados { get; set; }

        public ADO ADO { get; set; }

        public ADM ADM { get; set; }

        public ADMFRM ADMFRM { get; set; }

        public SAN SAN { get; set; }

        public PUSU1 PUSU1 { get; set; }
    }

    public class SYS
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class REG
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class ANTE
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class BEN
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class ADO
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class ADM
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class ADMFRM
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class SAN
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public class PUSU1
    {
        public int IdTipoPapel { get; set; }
        public int IdParticipante { get; set; }
    }

    public partial class PendenteFornecedor
    {
        [JsonProperty("Chamado Nº")]
        public string ChamadoNumero { get; set; }
    }
  
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
