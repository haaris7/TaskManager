import { ChevronDown } from 'lucide-react';
import { cn } from '../../utils/helpers';

const Select = ({ label, options, error, className, ...props }) => (
  <div className="space-y-1.5 w-full text-left">
    {label && <label className="text-sm font-semibold text-slate-600 ml-1">{label}</label>}
    <div className="relative">
      <select
        className={cn(
          "w-full bg-white/60 border border-white/50 rounded-xl px-4 py-3 text-slate-800 focus:outline-none focus:ring-2 focus:ring-sky-500/50 focus:border-sky-500/50 transition-all backdrop-blur-sm appearance-none cursor-pointer shadow-sm",
          error && "border-rose-500/50",
          className
        )}
        {...props}
      >
        {options.map(opt => (
          <option key={opt.value} value={opt.value} className="bg-white text-slate-800">
            {opt.label}
          </option>
        ))}
      </select>
      <ChevronDown className="absolute right-4 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none" size={16} />
    </div>
  </div>
);

export default Select;