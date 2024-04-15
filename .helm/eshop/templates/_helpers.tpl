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

{{/*
Scheme for Employee Authorization Server OAuth Issuer
*/}}
{{- define "employee-authorization-server.OAuthIssuer" -}}
{{- if .Values.employee_external_authserver.issuer  -}}
{{- .Values.employee_external_authserver.issuer }}
{{- else }}
{{- printf "https://%s/authorize" .Values.eshop_ingress.empoyeePortalDnsName }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Authorization Server OAuth Metadata Address
*/}}
{{- define "employee-authorization-server.OAuthMetadataAddress" -}}
{{- if .Values.employee_external_authserver.metadataAddress -}}
{{- .Values.employee_external_authserver.metadataAddress }}
{{- else }}
{{- printf "http://%s:%s/authorize/.well-known/openid-configuration" .Values.employeemanagement_authserver.appName (.Values.employeemanagement_authserver.servicePort | toString) }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Authorization Server OAuth Audience
*/}}
{{- define "employee-authorization-server.OAuthAudience" -}}
{{- if .Values.employee_external_authserver.audience -}}
{{- .Values.employee_external_authserver.audience }}
{{- else }}
{{- printf "" }}
{{- end -}}
{{- end -}}