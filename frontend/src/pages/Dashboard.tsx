import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { employeeAPI } from '../services/api';
import EmployeeTable from '../components/EmployeeTable';
import EmployeeForm from '../components/EmployeeForm';
import '../styles/dashboard.css';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadEmployees();
  }, []);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      const response = await employeeAPI.getAll();
      setEmployees(response.data);
    } catch (error) {
      console.error('Error loading employees:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (term: string) => {
    setSearchTerm(term);
    if (term.trim() === '') {
      loadEmployees();
      return;
    }
    
    try {
      const response = await employeeAPI.search(term);
      setEmployees(response.data);
    } catch (error) {
      console.error('Error searching employees:', error);
    }
  };

  const handleAddNew = () => {
    setSelectedEmployee(null);
    setShowForm(true);
  };

  const handleEdit = (employee: any) => {
    setSelectedEmployee(employee);
    setShowForm(true);
  };

  const handleSave = async () => {
    await loadEmployees();
    setShowForm(false);
    setSelectedEmployee(null);
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Sei sicuro di voler eliminare questo record?')) {
      try {
        await employeeAPI.delete(id);
        await loadEmployees();
      } catch (error) {
        console.error('Error deleting employee:', error);
      }
    }
  };

  const handleExport = async () => {
    try {
      const response = await employeeAPI.export();
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Dipendenti_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
    } catch (error) {
      console.error('Error exporting employees:', error);
    }
  };

  const handleImport = async (file: File) => {
    try {
      const response = await employeeAPI.import(file);
      console.log('Import result:', response.data);
      await loadEmployees();
      alert(`${response.data.data.length} dipendenti importati con successo`);
    } catch (error) {
      console.error('Error importing employees:', error);
      alert('Errore durante l\'importazione');
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <div>
          <h1>Nuova Previdenza</h1>
          <p>Gestione Dipendenti</p>
        </div>
        <div className="user-info">
          <span>Benvenuto, {user?.firstName} {user?.lastName}</span>
          <button onClick={handleLogout} className="btn-logout">Logout</button>
        </div>
      </header>

      <div className="dashboard-content">
        <div className="controls">
          <div className="search-box">
            <input
              type="text"
              placeholder="Cerca per nome o codice..."
              value={searchTerm}
              onChange={(e) => handleSearch(e.target.value)}
            />
          </div>

          <div className="buttons">
            <button onClick={handleAddNew} className="btn btn-primary">➕ Nuovo Dipendente</button>
            <button onClick={handleExport} className="btn btn-success">📥 Esporta Excel</button>
            <label className="btn btn-info">
              📤 Importa Excel
              <input
                type="file"
                accept=".xlsx"
                style={{ display: 'none' }}
                onChange={(e) => {
                  if (e.target.files?.[0]) {
                    handleImport(e.target.files[0]);
                  }
                }}
              />
            </label>
          </div>
        </div>

        {showForm && (
          <EmployeeForm
            employee={selectedEmployee}
            onSave={handleSave}
            onCancel={() => setShowForm(false)}
          />
        )}

        {!showForm && (
          <EmployeeTable
            employees={employees}
            loading={loading}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
        )}
      </div>
    </div>
  );
};

export default Dashboard;
