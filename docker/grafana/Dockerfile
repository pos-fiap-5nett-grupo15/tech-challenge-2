FROM grafana/grafana:latest

COPY prometheus-datasource.yml /etc/grafana/provisioning/datasources/prometheus-datasource.yml
COPY dashboard_config.json /var/lib/grafana/dashboards/dashboard.json

ENV GF_PATHS_PROVISIONING=/etc/grafana/provisioning
ENV GF_DASHBOARDS_DEFAULT_HOME=/var/lib/grafana/dashboards

ENV GF_SECURITY_ADMIN_USER=admin
ENV GF_SECURITY_ADMIN_PASSWORD=admin01
ENV GF_USERS_ALLOW_SIGN_UP=false

EXPOSE 3000

ENTRYPOINT ["/run.sh"]