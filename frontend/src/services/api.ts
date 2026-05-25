import axios from 'axios';

const API_URL = 'http://localhost:61355/api';

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authAPI = {
  register: (data: any) => apiClient.post('/auth/register', data),
  login: (data: any) => apiClient.post('/auth/login', data),
};

export const employeeAPI = {
  getAll: () => apiClient.get('/employees'),
  getById: (id: number) => apiClient.get(`/employees/${id}`),
  search: (term: string) => apiClient.get(`/employees/search/${term}`),
  create: (data: any) => apiClient.post('/employees', data),
  update: (id: number, data: any) => apiClient.put(`/employees/${id}`, data),
  delete: (id: number) => apiClient.delete(`/employees/${id}`),
  export: () => apiClient.get('/employees/export', { responseType: 'blob' }),
  import: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post('/employees/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};
