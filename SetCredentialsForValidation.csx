// This script replaces all Power Query partitions on this model with a
// The script assumes that all Power Query partitions load data from the same SQL Server-based data source.

// This function will extract all quoted values from the M expression, returning a list of strings
// with the values extracted (in order), but ignoring any quoted values where a hashtag (#) precedes
// the quotation mark:
var split = new Func<string, List<string>>(m => {
    var result = new List<string>();
    var i = 0;
    foreach (var s in m.Split('"'))
    {
        if (s.EndsWith("#") && i % 2 == 0) i = -2;
        if (i >= 0 && i % 2 == 1) result.Add(s);
        i++;
    }
    return result;
});
var GetServer = new Func<string, string>(m => split(m)[0]);    // Server name is usually the 1st encountered string
var GetDatabase = new Func<string, string>(m => split(m)[1]);  // Database name is usually the 2nd encountered string
var GetSchema = new Func<string, string>(m => split(m)[2]);    // Schema name is usually the 3rd encountered string
var GetTable = new Func<string, string>(m => split(m)[3]);     // Table name is usually the 4th encountered string

// Remove Power Query partitions from all tables and replace them with a single Legacy partition:
foreach (var t in Model.Tables.Where(t => t.Partitions.OfType<MPartition>().Any()))
{
    var mPartitions = t.Partitions.OfType<MPartition>();
    if (!mPartitions.Any()) continue;

    var schema = GetSchema(mPartitions.First().Expression);
    var table = GetTable(mPartitions.First().Expression);

    t.AddPartition(t.Name, string.Format("SELECT * FROM [{0}].[{1}]", schema, table));
    foreach (var p in mPartitions.ToList()) p.Delete();
}