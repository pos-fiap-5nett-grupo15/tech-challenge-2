FROM grupo15fiap.azurecr.io/grafana/grafana:11.2.0


RUN mkdir -p /etc/grafana/provisioning

WORKDIR /etc/grafana/provisioning/

COPY ./docker/grafana-image/dashboard_setup.json ./dashboards/imported-dashboards/dashboard.json
COPY ./docker/grafana-image/prometheus-datasource.yml ./datasources/prometheus-datasource.yml
COPY ./docker/grafana-image/prometheus-dashboard.yml ./dashboards/prometheus-dashboard.yml

WORKDIR /usr/share/grafana
ENTRYPOINT ["/run.sh"]