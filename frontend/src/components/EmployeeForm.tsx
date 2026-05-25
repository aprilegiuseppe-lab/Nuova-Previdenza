import React, { useState, useEffect } from 'react';
import { employeeAPI } from '../services/api';
import '../styles/form.css';

interface EmployeeFormProps {
  employee: any | null;
  onSave: () => void;
  onCancel: () => void;
}

const EmployeeForm: React.FC<EmployeeFormProps> = ({ employee, onSave, onCancel }) => {
  const [formData, setFormData] = useState({
    name: '',
    matricola: 0,
    code: '',
    totalAmount: 0,
    instalmentAmount: 0,
    numberOfInstalments: 0,
    decreeDate: '',
    decreeProtocol: '',
    notificationDate: '',
    dueDate: '',
    status: '',
    result: '',
    resultDate: '',
    plannedDecree: '',
    trafficLight: '',
    priority: '',
    witholdingProtocol: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (employee) {
      setFormData({
        ...employee,
        decreeDate: employee.decreeDate ? employee.decreeDate.split('T')[0] : '',
        notificationDate: employee.notificationDate ? employee.notificationDate.split('T')[0] : '',
        dueDate: employee.dueDate ? employee.dueDate.split('T')[0] : '',
        resultDate: employee.resultDate ? employee.resultDate.split('T')[0] : '',
      });
    }
  }, [employee]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name.includes('Amount') || name === 'numberOfInstalments' || name === 'matricola'
        ? parseFloat(value) || 0
        : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (employee) {
        await employeeAPI.update(employee.id, formData);
      } else {
        await employeeAPI.create(formData);
      }
      onSave();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Errore durante il salvataggio');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container">
      <h2>{employee ? 'Modifica Dipendente' : 'Nuovo Dipendente'}</h2>
      
      {error && <div className="error-message">{error}</div>}
      
      <form onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="form-group">
            <label>Nome Dipendente *</label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Matricola *</label>
            <input
              type="number"
              name="matricola"
              value={formData.matricola}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Codice *</label>
            <input
              type="text"
              name="code"
              value={formData.code}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Importo Totale *</label>
            <input
              type="number"
              step="0.01"
              name="totalAmount"
              value={formData.totalAmount}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Importo Rata *</label>
            <input
              type="number"
              step="0.01"
              name="instalmentAmount"
              value={formData.instalmentAmount}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Numero Rate *</label>
            <input
              type="number"
              name="numberOfInstalments"
              value={formData.numberOfInstalments}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Data Decreto *</label>
            <input
              type="date"
              name="decreeDate"
              value={formData.decreeDate}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Protocollo Decreto</label>
            <input
              type="text"
              name="decreeProtocol"
              value={formData.decreeProtocol}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label>Data Notifica *</label>
            <input
              type="date"
              name="notificationDate"
              value={formData.notificationDate}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Scadenza *</label>
            <input
              type="date"
              name="dueDate"
              value={formData.dueDate}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Stato</label>
            <select name="status" value={formData.status} onChange={handleChange}>
              <option value="">Seleziona...</option>
              <option value="Notificata">Notificata</option>
              <option value="Sospesa">Sospesa</option>
              <option value="Completata">Completata</option>
            </select>
          </div>

          <div className="form-group">
            <label>Esito</label>
            <select name="result" value={formData.result} onChange={handleChange}>
              <option value="">Seleziona...</option>
              <option value="ACCETTATO RATEALE">ACCETTATO RATEALE</option>
              <option value="RIFIUTATO">RIFIUTATO</option>
              <option value="IN SOSPESO">IN SOSPESO</option>
            </select>
          </div>

          <div className="form-group">
            <label>Data Esito</label>
            <input
              type="date"
              name="resultDate"
              value={formData.resultDate}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label>Decreto Previsto</label>
            <input
              type="text"
              name="plannedDecree"
              value={formData.plannedDecree}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label>Semaforo</label>
            <select name="trafficLight" value={formData.trafficLight} onChange={handleChange}>
              <option value="">Seleziona...</option>
              <option value="VERDE">🟢 Verde</option>
              <option value="GIALLO">🟡 Giallo</option>
              <option value="ROSSO">🔴 Rosso</option>
            </select>
          </div>

          <div className="form-group">
            <label>Priorità</label>
            <select name="priority" value={formData.priority} onChange={handleChange}>
              <option value="">Seleziona...</option>
              <option value="ALTA">ALTA</option>
              <option value="MEDIA">MEDIA</option>
              <option value="BASSA">BASSA</option>
            </select>
          </div>

          <div className="form-group">
            <label>Protocollo Trattenute</label>
            <input
              type="text"
              name="witholdingProtocol"
              value={formData.witholdingProtocol}
              onChange={handleChange}
            />
          </div>
        </div>

        <div className="form-actions">
          <button type="submit" disabled={loading} className="btn btn-primary">
            {loading ? 'Salvataggio...' : 'Salva'}
          </button>
          <button type="button" onClick={onCancel} className="btn btn-secondary">
            Annulla
          </button>
        </div>
      </form>
    </div>
  );
};

export default EmployeeForm;
