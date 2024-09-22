{{/*
Scheme for Catalog Api DB Connection String 
*/}}
{{- define "catalog-api.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.catalog.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.catalogSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.catalog.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

  
{{/*
Scheme for Customer Authorization Server DB Connection String
*/}}
{{- define "customer-authserver.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.customer.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.customerSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.customer.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Management DB Connection String
*/}}
{{- define "employee-management.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.employee.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.employeeSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.employee.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Ordering DB Connection String
*/}}
{{- define "ordering-api.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.ordering.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.orderingSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.ordering.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Saga Processor DB Connection String
*/}}
{{- define "saga-processor.connectionString" -}}
{{- if .Values.useAks -}}
{{- printf "Server=tcp:%s.database.windows.net,1433;Initial Catalog=eshop.saga.Db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";" .Values.azure.sagaSqlServerName }}
{{- else }}
{{- printf "Server=tcp:%s;Integrated Security=false;Initial Catalog=eshop.saga.Db;User Id=sa;password=%s;TrustServerCertificate=true" .Values.sql_db.appName .Values.sql_db.env.mssqlPassword }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Authorization Server OAuth Issuer
*/}}
{{- define "employee-authorization-server.OAuthIssuer" -}}
{{- if .Values.employee_external_authserver.issuer  -}}
{{- .Values.employee_external_authserver.issuer }}
{{- else }}
{{- printf "https://%s/authorize" .Values.eshop_ingress.eshopEmployeePortalDnsName }}
{{- end -}}
{{- end -}}

{{/*
Scheme for Employee Authorization Server OAuth Metadata Address
*/}}
{{- define "employee-authorization-server.OAuthMetadataAddress" -}}
{{- if .Values.employee_external_authserver.metadataAddress -}}
{{- .Values.employee_external_authserver.metadataAddress }}
{{- else }}
{{- printf "http://%s:%s/authorize/.well-known/openid-configuration" .Values.employeemanagement_authserver.appName ( 80 | toString) }}
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