﻿
using Oragon.Configuration;
using Oragon.Spring.Objects.Factory.Attributes;
using Oragon.Spring.Threading;
using System;
using System.Collections.Generic;
using NH = NHibernate;

namespace Oragon.Contexts.NHibernate
{
	/// <summary>
	///     Responsável por inicializar a configuraçãio do NHibernate e disponibilizar um SessionFactory pra a aplicação
	/// </summary>
	public abstract class AbstractSessionFactoryBuilder : ISessionFactoryBuilder
	{
		#region Injeção de Dependência

		/// <summary>
		///     Define o default Batch Size
		/// </summary>
		[Required]
		public int BatchSize { get; set; }

		/// <summary>
		///     Define o timeout padrão para a execução de comandos
		/// </summary>
		[Required]
		public int CommandTimeout { get; set; }

		/// <summary>
		///     Identifica qual a chave da conexão
		/// </summary>
		[Required]
		public IConfigurationResolver ConnectionStringDiscoverer { get; set; }

        /// <summary>
        ///     Define a estratégia de flush para conexões não transacionadas
        /// </summary>
		[Required]
		public NH.FlushMode DefaultFlushMode { get; set; }

		/// <summary>
		///     Define o nivel de isolamento da sessão
		/// </summary>
		[Required]
		public System.Data.IsolationLevel DefaultIsolationLevel { get; set; }

		[Required]
		public bool EnabledDiagnostics { get; set; }

		/// <summary>
		///     Define a profundidade máxima para o preenchimento automático do mecanismo de persistência.
		/// </summary>
		[Required]
		public int MaxFetchDepth { get; set; }

		/// <summary>
		///     Define uma chave para acesso ao banco
		/// </summary>
		[Required]
		public string ObjectContextKey { get; set; }

        /// <summary>
        ///     Define a estratégia de flush para conexões transacionadas
        /// </summary>
		[Required]
		public NH.FlushMode TransactionFlushMode { get; set; }

		[Required]
		public System.Data.IsolationLevel TransactionIsolationLevel { get; set; }

		/// <summary>
		///     Identifica tipos contidos para mapeamento
		/// </summary>
		[Required]
		public List<string> TypeNames { get; set; }

        public Dictionary<string, string> NHibernateRawConfigurationValues { get; set; }


        /// <summary>
		///     Identifica tipos contidos para mapeamento
		/// </summary>
        public string DefaultSchema { get; set; }

		#endregion Injeção de Dependência

		#region Instance State

		private readonly Semaphore semaphore = new Semaphore(1);
		private volatile NH.ISessionFactory sessionFactory;

		#endregion Instance State

		#region Public Constructors

		protected AbstractSessionFactoryBuilder()
		{
		}

		#endregion Public Constructors

		#region Métodos Públicos

		public NH.ISessionFactory BuildSessionFactory()
		{
			if (this.sessionFactory == null)
			{
				semaphore.Acquire();
				if (this.sessionFactory == null)//Reteste.. outra thread pode ter feito o preenchimento do campo antes da liberação do semáfoto.
				{
					try
					{
						this.sessionFactory = this.BuildSessionFactoryInternal();
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						semaphore.Release();
					}
				}
				else
					semaphore.Release();
			}
			return this.sessionFactory;
		}

		#endregion Métodos Públicos

		#region Métodos Privados

		/// <summary>
		///     Principal método privado, realiza a criação do SessionFactory e este não deve ser criado novamente até que o domínio de aplicação seja finalizado.
		/// </summary>
		/// <returns></returns>
		protected abstract NH.ISessionFactory BuildSessionFactoryInternal();

		#endregion Métodos Privados
	}
}