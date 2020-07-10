using AbrirChamado;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeletarUsuario
{
    class Program
    {
        public static int IdRequisicao { get; set; }

        #region Método Principal chamado na execução do programa
        public static void Main(string[] args)
        {
            //GetLog();
            GetTicketsApi();
            GetTicketsApiForm();
        }
        #endregion

        #region Método que busca no diretório os arquivos .txt que contém as informações de exclusão de usuário e valida se exclui ou não
        private static void GetLog()
        {
            string origem = ConfigurationManager.AppSettings["diretorio-log"];

            string[] fileExcluirUsuario = Directory.GetFiles(origem);

            foreach (var item in fileExcluirUsuario)
            {
                if (item.Contains(".txt"))
                {
                    bool fecharChamado = false;

                    var file = File.ReadAllText(item);

                    Log log = JsonConvert.DeserializeObject<Log>(file);

                    if (DateTime.Now > log.DataExclusao)
                    {
                        string idTarExcluscaoAcesso = ConfigurationManager.AppSettings["codigo-tarefa-exclusao-acesso"].ToString();
                        string idTarCodigoTarefa = ConfigurationManager.AppSettings["codigo-tarefa"].ToString();
                        string descricao = log.Descricao + "\n Conforme Chamado matriz nº " + log.Id;
                        string idParticipante = log.IdParticipante;

                        AbrirChamado.RootObject chamadoExclusaoAcesso = MontarChamado(descricao, log.Id, idTarExcluscaoAcesso, idParticipante);
                        AbrirTicket(chamadoExclusaoAcesso, fecharChamado);

                        string retornoRemover = RemoverUsuario(log.Login);

                        if (retornoRemover != "")
                        {
                            fecharChamado = false;
                            AbrirChamado.RootObject chamado = MontarChamado(retornoRemover, log.Id, idTarExcluscaoAcesso,  idParticipante);
                            AbrirTicket(chamado, fecharChamado);
                        }
                        else
                        {
                            string usuarioRemovido = "Remover usuario " + log.Login + "\n Conforme Chamado matriz nº " + log.Id +
                                                 "\n Chamado nº " + IdRequisicao + " foi aberto para exclusão de sistema.";
                            fecharChamado = true;

                            AbrirChamado.RootObject chamadoUsuarioRemovido = MontarChamado(usuarioRemovido, log.Id, idTarCodigoTarefa, idParticipante);
                            AbrirTicket(chamadoUsuarioRemovido, fecharChamado);
                        }

                        File.Delete(item);
                    }
                }
            }
        }
        #endregion

        #region Monta o corpo do chamado para ser enviado via POST na API do ASK
        private static AbrirChamado.RootObject MontarChamado(string descricao, string id, string idTar, string idParticipante)
        {
            AbrirChamado.RootObject body = new AbrirChamado.RootObject();
            body.Conjuntos = new AbrirChamado.Conjuntos();
            body.Conjuntos.DadosBasicos = new DadosBasicos();
            body.Conjuntos.Papeis = new Papeis();

            body.Formulario = 68;
            body.Conjuntos.DadosBasicos.Assunto = "Liberação de Acessos - Conta de rede - Excluir Conta de Rede - Automatizado";
            body.Conjuntos.DadosBasicos.Descricao = descricao;
            body.Conjuntos.DadosBasicos.DescricaoOrigem = "Chatbot";
            body.Conjuntos.DadosBasicos.Atividade = idTar;
            body.Conjuntos.Papeis.ANTE = "{ IdParticipante : " + idParticipante + ", IdTipoPapel : 1 }";

            return body;
        }
        #endregion

        #region Abre chamado
        private static void AbrirTicket(AbrirChamado.RootObject body, bool fecharChamado)
        {
            string api = ConfigurationManager.AppSettings["api-url-abrir"].ToString();

            var URL = new Uri(api);

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) })
            {
                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());
                var serializedProduto = JsonConvert.SerializeObject(body);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol =
                   SecurityProtocolType.Ssl3 |
                   SecurityProtocolType.Tls |
                   SecurityProtocolType.Tls11 |
                   SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.PostAsync(URL, content).Result)
                    {
                        var retornoIdChamado = response.Content.ReadAsStringAsync().Result;
                        string id = retornoIdChamado.Replace("\"", "");

                        if (response.IsSuccessStatusCode && fecharChamado == false)
                        {
                            GerarLogTicketAberto(id, body.Conjuntos.DadosBasicos.Descricao);
                            IdRequisicao = Convert.ToInt32(id);
                        }
                        else if (response.IsSuccessStatusCode && fecharChamado == true)
                        {
                            ConcluirTicket(id, body.Conjuntos.DadosBasicos.Descricao);
                        }
                        else
                        {
                            Console.WriteLine("Não foi possível abrir o chamado: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exception = "Não foi possível abrir o chamado: " + ex.InnerException;
                    Console.WriteLine(exception);
                    TratarExcecao(ex);
                }
            }
        }
        #endregion

        #region Conclui o chamado enviando um JSON via Post para a API do ASK e gera arquivo de log no diretorio Log-Chamados-Encerrados
        private static void ConcluirTicket(string id, string descricao)
        {
            string api = ConfigurationManager.AppSettings["api-url"].ToString();
            string destino = ConfigurationManager.AppSettings["diretorio-log"] + "Log-Chamados-Encerrados\\";

            var URL = new Uri(api + "/requisicoes/" + id + "/acoes");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) })
            {
                ConcluirTarefa concluir = new ConcluirTarefa();

                concluir.Id = "Concluir Requisição [CONC]";
                concluir.Descricao = "Olá, sou a Gabi - concluí o chamado para " + descricao;

                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());
                var serializedProduto = JsonConvert.SerializeObject(concluir);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol =
                   SecurityProtocolType.Ssl3 |
                   SecurityProtocolType.Tls |
                   SecurityProtocolType.Tls11 |
                   SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.PostAsync(URL, content).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Requisição " + id + " concluída com sucesso!");
                            File.WriteAllText(destino + id + " - Chamado concluído.txt", concluir.Descricao);

                            //TO DO: Implementar método abaixo apenas em produção
                            //Email.EnviarEmailSucesso("chamado - " + ticket.Id + ticket.Descricao);
                        }
                        else
                        {
                            Console.WriteLine("Não foi possível concluir o chamado: " + response.StatusCode);
                            descricao += " - Não foi concluído, erro ao fazer POST na API.";
                            File.WriteAllText(destino + "Não foi possível concluir o chamado - " + id + ".txt", descricao);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exception = "Não foi possível concluir o chamado: " + ex.InnerException;
                    Console.WriteLine(exception);
                    //TratarExcecao(ex);
                }
            }
        }
        #endregion

        #region Cria arquivo JSON com os detalhes do chamado
        public static void GerarLogTicketAberto(string id, string detalhe)
        {
            string destino = ConfigurationManager.AppSettings["diretorio-log"].ToString() + "Log-Chamados-Abertos\\";
            ConcluirTarefa chamado = new ConcluirTarefa();
            chamado.Id = id;
            chamado.Descricao = detalhe;

            string json = JsonConvert.SerializeObject(chamado);

            File.WriteAllText(destino + id + " - Chamado aberto.txt", json);
        }
        #endregion

        #region Consulta todos os chamados com status "em aberto" através de um Post na API que são abertos via email SAP
        private static async void GetTicketsApi()
        {
            var URL = new Uri(ConfigurationManager.AppSettings["api-url"].ToString() + "/requisicoes/abertas");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(40) })
            {
                CodigoTarefa codTarefa = new CodigoTarefa();

                codTarefa.AtividadeId = Convert.ToInt32(ConfigurationManager.AppSettings["codigo-tarefa"]);
                codTarefa.Status = "Em Aberto";

                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());
                var serializedProduto = JsonConvert.SerializeObject(codTarefa);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol = 
                    SecurityProtocolType.Ssl3 | 
                    SecurityProtocolType.Tls | 
                    SecurityProtocolType.Tls11 | 
                    SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.PostAsync(URL, content).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var requisicoes = await response.Content.ReadAsStringAsync();

                            RootObject Requisicoes = JsonConvert.DeserializeObject<RootObject>(requisicoes);

                            foreach (var item in Requisicoes.records)
                            {

                                if (item.Status == "Em Aberto")
                                {
                                    Console.WriteLine(item.RequisicaoId);
                                    Console.WriteLine(item.RequisicaoId);

                                    IdRequisicao = item.RequisicaoId;
                                    RequisicaoDetalhes requisicaoDetalhes = BuscarDadosAdicionais(item.RequisicaoId);


                                    if (requisicaoDetalhes != null)
                                    {
                                        ExtrairUsuarioAD(requisicaoDetalhes);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Não foi possível obter os chamados: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TratarExcecao(ex);
                }
            }
        }
        #endregion

        #region Consulta todos os chamados com status "em aberto" através de um Post na API que são abertos no formulário do ASK
        private static async void GetTicketsApiForm()
        {
            var URL = new Uri(ConfigurationManager.AppSettings["api-url"].ToString() + "/requisicoes/abertas");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(40) })
            {
                CodigoTarefa codTarefa = new CodigoTarefa();

                codTarefa.AtividadeId = Convert.ToInt32(ConfigurationManager.AppSettings["codigo-tarefa-form"]);

                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());
                var serializedProduto = JsonConvert.SerializeObject(codTarefa);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol =
                                SecurityProtocolType.Ssl3 |
                                SecurityProtocolType.Tls |
                                SecurityProtocolType.Tls11 |
                                SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.PostAsync(URL, content).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var requisicoes = await response.Content.ReadAsStringAsync();

                            RootObject Requisicoes = JsonConvert.DeserializeObject<RootObject>(requisicoes);

                            foreach (var item in Requisicoes.records)
                            {
                                if (item.Status == "Em Aberto")
                                {
                                    Console.WriteLine(item.RequisicaoId);
                                    IdRequisicao = item.RequisicaoId;
                                    RequisicaoDetalhes requisicaoDetalhes = BuscarDadosAdicionais(item.RequisicaoId);

                                    if (requisicaoDetalhes != null)
                                    {
                                        string idParticipante = requisicaoDetalhes.Conjuntos.IdsParticipantes.SolicitanteDados.IdParticipante.ToString();
                                        string login = requisicaoDetalhes.Conjuntos.DadosUsuarioBloquear.Login;
                                        string nomeCompleto = requisicaoDetalhes.Conjuntos.DadosUsuarioBloquear.NomeCompleto;

                                        string memberOf = GetMemberOf(login);
                                        Console.WriteLine(memberOf);

                                        BloquearUsuario(login);

                                        if (memberOf != "")
                                        {
                                            string concluir = "Usuário bloqueado: " + nomeCompleto
                                                + " \n Membro dos seguintes grupos: \n " + memberOf.Replace(",", " \n");

                                            Console.WriteLine(IdRequisicao);
                                            Console.WriteLine(memberOf);
                                            Console.WriteLine(login);

                                            ConcluirTicket(IdRequisicao.ToString(), concluir);
                                            GerarLog(IdRequisicao, memberOf, login, idParticipante);
                                        }
                                        else
                                        {
                                            //string concluir = "Usuário não encontrado. Favor fornecer um login válido.";
                                            //ConcluirTicket(IdRequisicao.ToString(), concluir);

                                            RedirecionarTicket(IdRequisicao.ToString(), nomeCompleto);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Não foi possível obter os chamados: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TratarExcecao(ex);
                }
            }
        }

        private static void RedirecionarTicket(string id, string nomeCompleto)
        {
            string api = ConfigurationManager.AppSettings["api-url"].ToString();
            string destino = ConfigurationManager.AppSettings["diretorio-log"] + "Log-Chamados-Encerrados\\";

            var URL = new Uri(api + "/requisicoes/" + id + "/acoes");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) })
            {
                DirecionarTarefa direcionar = new DirecionarTarefa();

                direcionar.Id = "Direcionar Requisicao [DIR]";
                direcionar.Descricao = "Ei aqui é a Gabi, estava verificando aqui e infelizmente não localizei o usuário " + nomeCompleto +
                                      ". Por isso estou direcionando este chamado para ser atendido por uma pessoa.";
                direcionar.NovoSolicitado = "12889";
                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());
                var serializedProduto = JsonConvert.SerializeObject(direcionar);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol =
                   SecurityProtocolType.Ssl3 |
                   SecurityProtocolType.Tls |
                   SecurityProtocolType.Tls11 |
                   SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.PostAsync(URL, content).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Requisição " + id + " direcionada com sucesso!");
                            File.WriteAllText(destino + id + " - Chamado direcionado.txt", direcionar.Descricao);

                            //TO DO: Implementar método abaixo apenas em produção
                            //Email.EnviarEmailSucesso("chamado - " + ticket.Id + ticket.Descricao);
                        }
                        else
                        {
                            Console.WriteLine("Não foi possível direcionar o chamado: " + response.StatusCode);
                            direcionar.Descricao += " - Não foi direcionado, erro ao fazer POST na API.";
                            File.WriteAllText(destino + "Não foi possível direcionar o chamado - " + id + ".txt", direcionar.Descricao);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exception = "Não foi possível direcionar o chamado: " + ex.InnerException;
                    Console.WriteLine(exception);
                    //TratarExcecao(ex);
                }
            }
        }
        #endregion

        #region Consulta no AD o usuário que será excluído e chama o método de bloqueio 
        private static void ConsultarUsuario(string firstName, string lastName)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                try
                {
                    var formattedOutput = "Get-ADUser -Filter {GivenName -eq \"" + firstName + "\" -and Surname -eq \"" + lastName + "\"}";
                    //var formattedOutput = "Get-WmiObject -Class win32_computersystem -ComputerName NBINTELLI13";

                    Console.WriteLine(formattedOutput);

                    PowerShellInstance.AddScript(formattedOutput);

                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                    PowerShellInstance.Stop();

                    var name = "";
                    var login = "";

                    foreach (PSObject outputItem in PSOutput)
                    {
                        if (outputItem != null)
                        {
                            foreach (var item in outputItem.Members)
                            {
                                name = item.Name;

                                if (name == "SamAccountName")
                                {
                                    login = item.Value.ToString();
                                    Console.WriteLine(name);
                                    Console.WriteLine(login);
                                }
                            }

                        }
                    }

                    if (login != "")
                    {
                        string memberOf = GetMemberOf(login);
                        Console.WriteLine(memberOf);

                        BloquearUsuario(login);


                        if (memberOf != "")
                        {
                            string concluir = "Usuário bloqueado: " + firstName + " " + lastName
                                + " \n Membro dos seguintes grupos: \n " + memberOf.Replace(",", " \n");

                            ConcluirTicket(IdRequisicao.ToString(), concluir);
                            GerarLog(IdRequisicao, memberOf, login, "");
                        }
                    }
                    else
                    {
                        ConcluirTicket(IdRequisicao.ToString(), "Usuário " + firstName + " " + lastName + " não encontrado no AD.");
                    }
                }
                catch (Exception ex)
                {
                    TratarExcecao(ex);
                    Console.WriteLine("Não foi possível excluir o usuário: " + ex.Message);
                }
            }
        }
        #endregion

        #region Gera log para exclusão de permissões do usuário manualmente depois de bloqueado por 7 dias (regra de bloqueio GAB) no AD
        private static void GerarLog(int idRequisicao, string memberOf, string login, string idParticipante)
        {
            Log log = new Log();
            log.Id = idRequisicao.ToString();
            log.Login = login;
            log.Descricao = " \n Excluir as permissões do usuário " + login + " nos seguintes grupos e sistemas: \n " + memberOf.Replace(",", " \n");
            log.DataExclusao = DateTime.Now.AddDays(5);
            //log.DataExclusao = DateTime.Now.AddMinutes(1);
            log.IdParticipante = idParticipante;

            string json = JsonConvert.SerializeObject(log);

            File.WriteAllText(ConfigurationManager.AppSettings["diretorio-log"].ToString()
                    + idRequisicao + ".txt", json);
        }
        #endregion

        #region Cria o comando powershell de bloqueio de usuário no AD e executa
        private static void BloquearUsuario(string login)
        {
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    var formattedOutput = "Disable-ADAccount -Identity " + login;

                    Console.WriteLine(formattedOutput);

                    PowerShellInstance.AddScript(formattedOutput);
                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                    PowerShellInstance.Stop();
                }
            }
            catch (Exception ex)
            {
                TratarExcecao(ex);
            }
        }
        #endregion

        #region Cria o comando powershell de exclusão de usuário no AD e executa
        private static string RemoverUsuario(string login)
        {
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    var name = "";
                    var result = "";
                    var formattedOutput = "Remove-ADUser -Identity " + login + " -Confirm:$false";

                    Console.WriteLine(formattedOutput);

                    PowerShellInstance.AddScript(formattedOutput);
                    Collection<PSObject> pSOutput = PowerShellInstance.Invoke();
                    PowerShellInstance.Stop();

                    formattedOutput = "Get-ADUser -Identity " + login;

                    Console.WriteLine(formattedOutput);

                    PowerShellInstance.AddScript(formattedOutput);
                    pSOutput = PowerShellInstance.Invoke();
                    PowerShellInstance.Stop();

                    foreach (PSObject outputItem in pSOutput)
                    {
                        if (outputItem != null)
                        {
                            foreach (var item in outputItem.Members)
                            {
                                name = item.Name;

                                if (name == "Name")
                                {
                                    result += item.Value.ToString();
                                    Console.WriteLine(name);
                                    Console.WriteLine(result);
                                    return "O Usuário " + item.Value.ToString() + " não pode ser deletado pelo Chat-bot Gabi, favor deletar manualmente.";
                                }
                            }
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                TratarExcecao(ex);
                return "";
            }
        }
        #endregion

        #region Consulta no AD via comando powershell quais as permissões que o usuário tem na rede (grupos de acesso que faz parte)
        private static string GetMemberOf(string login)
        {
            var name = "";
            var MemberOf = "";

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                //var formattedOutput = "Get-ADUser -Identity \"" + login + "\" -Properties *";
                var formattedOutput = "Get-ADPrincipalGroupMembership " + login;

                Console.WriteLine(formattedOutput);
                //Console.ReadLine();

                PowerShellInstance.AddScript(formattedOutput);
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                PowerShellInstance.Stop();

                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem != null)
                    {
                        foreach (var item in outputItem.Members)
                        {
                            name = item.Name;

                            if (name == "SamAccountName")
                            {
                                MemberOf += item.Value.ToString() + ", ";
                                Console.WriteLine(name);
                                Console.WriteLine(MemberOf);
                                //Console.ReadLine();
                            }
                        }
                    }
                }
            }

            return MemberOf;
        }
        #endregion

        //private static void ExtrairUsuarioAD(RequisicaoDetalhes requisicaoDetalhes)
        //{
        //    var arrayDescricao = requisicaoDetalhes.Conjuntos.DetalhesDoPedido.Descricao.Split(new string[] { "\r" },
        //        StringSplitOptions.None);

        //    string nome = "";
        //    string[] ns = null;
        //    string firstName = "";
        //    string lastName = "";

        //    foreach (var item in arrayDescricao)
        //    {
        //        if (item.Contains("Empregado"))
        //        {
        //            string linha = item;
        //            int h = linha.IndexOf('-') + 1;
        //            int f = linha.IndexOf('\"');

        //            nome = linha.Substring(h, f - h).Trim();
        //            ns = nome.Split(' ');
        //            firstName = ns[0];
        //            lastName = String.Join(" ", ns, 1, ns.Length - 1);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
        //    {
        //        ConsultarUsuario(firstName, lastName);
        //    }
        //}

        #region Lógica que extrai os dados do usuário que vem na descrição do chamado para depois consultar no AD
        private static void ExtrairUsuarioAD(RequisicaoDetalhes requisicaoDetalhes)
        {
            string[] ns = null;
            string firstName = "";
            string lastName = "";
            string empregadoNome = "";


            string email = requisicaoDetalhes.Conjuntos.DetalhesDoPedido.Descricao;
            List<string> sujeira = new List<string>() { "<br/>", "<br />", "\r" };
            foreach (string s in sujeira)
            {
                email = email.Replace(s, "");
            }

            string verificaSolicitacao = "Cód. Solicitação:";
            if (email.Contains(verificaSolicitacao))
            {
                Console.WriteLine("passou aqui");

                string msgSolicitacao = email.Substring(email.IndexOf(verificaSolicitacao));
                Console.WriteLine(msgSolicitacao);
                string[] linhas = msgSolicitacao.Split('\n');
                string empregado = "";
                foreach (string linha in linhas)
                {
                    if (!String.IsNullOrWhiteSpace(linha) && linha.Contains(":"))
                    {
                        string[] fields = linha.Split(':');
                        if (fields[0] == "Empregado")
                        {
                            empregado = fields[1];
                            empregadoNome = empregado.Split('-')[1].Trim();
                        }
                    }
                }

                ns = empregadoNome.Split(' ');
                firstName = ns[0];
                lastName = String.Join(" ", ns, 1, ns.Length - 1);

                Console.WriteLine("O Empregado da solicitação é: " + empregadoNome);

                Console.WriteLine("Nome: " + firstName);
                Console.WriteLine("Sobrenome: " + lastName);
            }

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                ConsultarUsuario(firstName, lastName);
            }
        }
        #endregion

        #region Consulta na API do ASK os dados adicionais do chamado
        private static RequisicaoDetalhes BuscarDadosAdicionais(int id)
        {
            var URL = ConfigurationManager.AppSettings["api-url"].ToString() + "/requisicoes/" + id.ToString();

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(35) })
            {
                client.DefaultRequestHeaders.Add("Authorization", ConfigurationManager.AppSettings["bearer"].ToString());

                ServicePointManager.SecurityProtocol =
                                SecurityProtocolType.Ssl3 |
                                SecurityProtocolType.Tls |
                                SecurityProtocolType.Tls11 |
                                SecurityProtocolType.Tls12;

                try
                {
                    using (var response = client.GetAsync(URL).Result)
                    {
                        var detalhes = response.Content.ReadAsStringAsync().Result;

                        RequisicaoDetalhes requisicaoDetalhes = JsonConvert.DeserializeObject<RequisicaoDetalhes>(detalhes);
                        DadosDoCliente dadosCliente = requisicaoDetalhes.Conjuntos.DadosDoCliente;

                        return requisicaoDetalhes;
                    }
                }
                catch (Exception ex)
                {
                    TratarExcecao(ex);
                }

                return null;
            }
        }
        #endregion

        #region Tratamento de exceção e envio de email reportando o erro
        private static void TratarExcecao(Exception ex)
        {
            string excecao = "";

            if (ex.InnerException != null)
            {
                excecao = ex.InnerException.ToString();
            }

            if (excecao.Contains("A task was canceled"))
            {
                excecao = "Erro de TimeOut";
            }

            string erro = "Não foi possível obter os dados da API - " + excecao;
            Console.WriteLine(erro);
            //Email.EnviarEmail(erro);
        }
        #endregion
    }
}
