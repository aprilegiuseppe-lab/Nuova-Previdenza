import React from 'react';
import { format } from 'date-fns';
import { it } from 'date-fns/locale';
import '../styles/table.css';

interface EmployeeTableProps {
  employees: any[];
  loading: boolean;
  onEdit: (employee: any) => void;
  onDelete: (id: number) => void;
}

const EmployeeTable: React.FC<EmployeeTableProps> = ({
  employees,
  loading,
  onEdit,
  onDelete,
}) => {
  if (loading) {
    return <div className="loading">Caricamento...</div>;
  }

  if (employees.length === 0) {
    return <div className="no-data">Nessun dipendente trovato</div>;
  }

  return (
    <div className="table-container">
      <table className="employee-table">
        <thead>
          <tr>
            <th>Dipendente</th>
            <th>Matricola</th>
            <th>Codice</th>
            <th>Importo Tot</th>
            <th>Importo Rata</th>
            <th>N. Rate</th>
            <th>Data Decreto</th>
            <th>Stato</th>
            <th>Esito</th>
            <th>Priorità</th>
            <th>Azioni</th>
          </tr>
        </thead>
        <tbody>
          {employees.map((employee) => (
            <tr key={employee.id}>
              <td>{employee.name}</td>
              <td>{employee.matricola}</td>
              <td>{employee.code}</td>
              <td className="amount">€ {employee.totalAmount?.toFixed(2) || '0.00'}</td>
              <td className="amount">€ {employee.instalmentAmount?.toFixed(2) || '0.00'}</td>
              <td className="center">{employee.numberOfInstalments}</td>
              <td>
                {employee.decreeDate ? format(new Date(employee.decreeDate), 'dd/MM/yyyy', {
                  locale: it,
                }) : '-'}
              </td>
              <td>
                <span className={`badge status-${employee.status?.toLowerCase()}`}>
                  {employee.status || '-'}
                </span>
              </td>
              <td>{employee.result || '-'}</td>
              <td>
                <span className={`priority priority-${employee.priority?.toLowerCase()}`}>
                  {employee.priority || '-'}
                </span>
              </td>
              <td className="actions">
                <button
                  onClick={() => onEdit(employee)}
                  className="btn-action btn-edit"
                  title="Modifica"
                >
                  ✏️
                </button>
                <button
                  onClick={() => onDelete(employee.id)}
                  className="btn-action btn-delete"
                  title="Elimina"
                >
                  🗑️
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default EmployeeTable;
