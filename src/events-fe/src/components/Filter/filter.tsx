import "./filter.css";

interface FilterProps {
  fromDate: string;
  toDate: string;
  onFromDateChange: (value: string) => void;
  onToDateChange: (value: string) => void;
  onFilter: () => void;
  maxDateTime?: string;
}

const Filter = (props: FilterProps) => {
  const { fromDate, toDate, onFromDateChange, onToDateChange, onFilter, maxDateTime } = props;
  return (
    <div className="filter-container">
      <div className="filter-group">
        <label className="filter-label">From:</label>
        <input
          type="datetime-local"
          step="60"
          max={maxDateTime}
          value={fromDate}
          onChange={(e) => onFromDateChange(e.target.value)}
          className="filter-input"
        />
      </div>
      <div className="filter-group">
        <label className="filter-label">To:</label>
        <input
          type="datetime-local"
          step="60"
          max={maxDateTime}
          value={toDate}
          onChange={(e) => onToDateChange(e.target.value)}
          className="filter-input"
        />
      </div>
      <button onClick={onFilter} className="filter-button">
        Filter
      </button>
    </div>
  );
};

export default Filter;