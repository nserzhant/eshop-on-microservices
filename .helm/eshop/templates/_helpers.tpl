{{/*
Scheme for Catalog Api DB Connection String 
*/}}
{{- define "catalog-api.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.catalog.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.catalogApiSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.catalog.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

  
{{/*
Scheme for Client Auth Server DB Connection String
*/}}
{{- define "client-authserver.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.client.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.clientAuthserverSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.client.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Management DB Connection String
*/}}
{{- define "employee-management.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.employee.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.employeeManagementSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.employee.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}