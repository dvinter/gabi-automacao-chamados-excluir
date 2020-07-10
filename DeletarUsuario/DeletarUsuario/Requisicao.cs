using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletarUsuario
{
    public class Metadata
    {
    }

    public class Records
    {
        public string Assunto { get; set; }
        public int RequisicaoId { get; set; }
        public int ProcessoId { get; set; }
        public int FormularioId { get; set; }
        public int AtividadeId { get; set; }
        public int FormularioStatusId { get; set; }
        public string Status { get; set; }
        public string DescricaoAtividade { get; set; }
        public string DescricaoRequisicao { get; set; }
        public string DescricaoRotinaOuFormulario { get; set; }
        public string UltimaAcao { get; set; }
        public DateTime? UltimaAcaoQuando { get; set; }
        public string UltimaAcaoQuem { get; set; }
        public string Solicitante { get; set; }
        public string Solicitado { get; set; }
        public string Beneficiado { get; set; }
        public string Responsavel { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataInicioPrevisto { get; set; }
        public DateTime? DataFim { get; set; }
        public DateTime? DataFimPrevisto { get; set; }
        public string DataInicioReal { get; set; }
        public string DataFimReal { get; set; }
        public int GrupoAtualId { get; set; }
        public string FlagSLA { get; set; }
        public string UrlDetalhesRequisicao { get; set; }
        public string UrlHistoricoRequisicao { get; set; }
    }

    public class RootObject
    {
        public Metadata _metadata { get; set; }
        public List<Records> records { get; set; }
    }
}
