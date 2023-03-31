//add a value pair for each parameter name and parameter value. 
//For values with quotes, use the escape character \ before the quote.
string[,] a = new string[,] {
     {"serverName","\"ppeserver.database.windows.net\""}, //set the config for each env
     {"databaseName","\"DataHub\""}, //ContosoPPEDb, ContosoProdDb
     {"StartDate", "#date(2017, 1, 1)"}, //#datetime(2022, 5, 1, 0, 0, 0), #datetime(2017, 1, 1, 0, 0, 0)
};

//DO NOT CHANGE ANYTHING BELOW THIS LINE
string paramExpression;
int paramMetaLoc;
string oldParameterVal;
string newParameterVal;
string newParamExpression;
int x, y;
foreach(var e in Model.Expressions)
{
    paramExpression = Model.Expressions[e.Name].Expression;
    paramMetaLoc = paramExpression.IndexOf(" meta [IsParameterQuery=true");
    if (paramMetaLoc > 0) {     //only do the rest if this is a parameter type query
        for (x = 0; x < a.GetLength(0); x++) {
            if (e.Name == a[x,0]) { //match up the parameters in the model with those defined above
                oldParameterVal = paramExpression.Left(paramMetaLoc);
                newParameterVal = a[x,1];
                newParamExpression = newParameterVal + paramExpression.Substring(paramMetaLoc);
                Model.Expressions[e.Name].Expression = newParamExpression;  //update the parameter
            }
        }
    }
}