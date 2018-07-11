using NHibernate.AdoNet;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMusicStore.Infrastructure.Persistence
{
    public class MySQLConnector57Dialect : MySQL57Dialect
    {

    }


    public class MySqlConnectorDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDataDriver"/> class.
        /// </summary>
        /// <exception cref="HibernateException">
        /// Thrown when the <c>MySql.Data</c> assembly can not be loaded.
        /// </exception>
        public MySqlConnectorDriver() : base(
            "MySql.Data.MySqlClient",
            "MySqlConnector",
            "MySql.Data.MySqlClient.MySqlConnection",
            "MySql.Data.MySqlClient.MySqlCommand")
        {
        }

        /// <summary>
        /// MySql.Data uses named parameters in the sql.
        /// </summary>
        /// <value><see langword="true" /> - MySql uses <c>?</c> in the sql.</value>
        public override bool UseNamedPrefixInSql => true;

        /// <summary></summary>
        public override bool UseNamedPrefixInParameter => true;

        /// <summary>
        /// MySql.Data use the <c>?</c> to locate parameters in sql.
        /// </summary>
        /// <value><c>?</c> is used to locate parameters in sql.</value>
        public override string NamedPrefix => "?";

        /// <summary>
        /// The MySql.Data driver does NOT support more than 1 open DbDataReader
        /// with only 1 DbConnection.
        /// </summary>
        /// <value><see langword="false" /> - it is not supported.</value>
        public override bool SupportsMultipleOpenReaders => false;

        /// <summary>
        /// MySql.Data does not support preparing of commands.
        /// </summary>
        /// <value><see langword="false" /> - it is not supported.</value>
        /// <remarks>
        /// With the Gamma MySql.Data provider it is throwing an exception with the 
        /// message "Expected End of data packet" when a select command is prepared.
        /// </remarks>
        protected override bool SupportsPreparingCommands => false;

        public override IResultSetsCommand GetResultSetsCommand(NHibernate.Engine.ISessionImplementor session)
        {
            return new BasicResultSetsCommand(session);
        }

        public override bool SupportsMultipleQueries => true;

        public override bool RequiresTimeSpanForTime => true;

        // As of v5.7, lower dates may "work" but without guarantees.
        // https://dev.mysql.com/doc/refman/5.7/en/datetime.html
        /// <inheritdoc />
        public override DateTime MinDate => new DateTime(1000, 1, 1);

        System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(MySqlClientBatchingBatcherFactory);
    }
}
