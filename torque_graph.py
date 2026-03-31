import sys
import pandas as pd
import plotly.graph_objects as go


def generate_torque_graph(file_path):
    df = pd.read_csv(file_path, sep=';', decimal=',')

    df.columns = [c.strip() for c in df.columns]

    min_torque_df = df.groupby('RPM')['Torque [Nm]'].min().reset_index().sort_values('RPM')
    max_torque_df = df.groupby('RPM')['Torque [Nm]'].max().reset_index().sort_values('RPM')

    fig = go.Figure()

    fig.add_trace(go.Scatter(
        x=df['RPM'],
        y=df['Torque [Nm]'],
        mode='markers',
        name='Torque points',
        marker=dict(color='rgba(100, 150, 250, 0.4)', size=6),
        hovertemplate=(
            "<b>RPM:</b> %{x}<br>" +
            "<b>Torque:</b> %{y} Nm<br>" +
            "<b>Power:</b> %{customdata[5]} HP<br>" +
            "<b>MAP:</b> %{customdata[0]} kPa<br>" +
            "<b>Fuel rate:</b> %{customdata[1]} L/h<br>" +
            "<b>Efficiency:</b> %{customdata[2]:.2%}<br>" +
            "<b>VE:</b> %{customdata[3]}<br>" +
            "<b>AFR:</b> %{customdata[4]}" +
            "<extra></extra>"
        ),
        customdata=df[['MAP [kPa]', 'Fuel Rate [L/h]', 'Efficiency', 'VE', 'AFR', 'Power [HP]']]
    ))

    fig.add_trace(go.Scatter(
        x=max_torque_df['RPM'],
        y=max_torque_df['Torque [Nm]'],
        mode='lines',
        name='Max torque',
        line=dict(color='firebrick', width=3)
    ))

    fig.add_trace(go.Scatter(
        x=min_torque_df['RPM'],
        y=min_torque_df['Torque [Nm]'],
        mode='lines',
        name='Min torque',
        line=dict(color='blue', width=3)
    ))

    fig.update_layout(
        title="Engine Torque Graph",
        xaxis_title="Engine RPM",
        yaxis_title="Torque [Nm]",
        template="plotly_dark",
        hovermode="closest"
    )

    fig.show()

if __name__ == "__main__":
    args = sys.argv
    csv_path = args[1] if len(args) > 1 else "result.csv"
    generate_torque_graph(csv_path)