FROM grupo15fiap.azurecr.io/grafana/grafana:11.2.0


RUN mkdir -p /etc/grafana/provisioning

WORKDIR /etc/grafana/provisioning/

COPY ./dashboard_setup.json ./dashboards/imported-dashboards/dashboard.json
COPY ./prometheus-datasource.yml ./datasources/prometheus-datasource.yml
COPY ./prometheus-dashboard.yml ./dashboards/prometheus-dashboard.yml

WORKDIR /usr/share/grafana
ENTRYPOINT ["/run.sh"]